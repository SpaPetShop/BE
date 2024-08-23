﻿using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Meta.BusinessTier.Constants;
using Meta.BusinessTier.Enums.EnumStatus;
using Meta.BusinessTier.Payload.Order;
using Meta.BusinessTier.Payload.Payment;
using Meta.BusinessTier.Services.Interfaces;
using Meta.BusinessTier.Utils;
using Meta.DataTier.Models;
using Meta.DataTier.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using Meta.BusinessTier.Enums.Status;

namespace Meta.BusinessTier.Services.Implements
{
    public class PaymentService : BaseService<PaymentService>, IPaymentService
    {
        private readonly IConfiguration _configuration;
        private readonly IOrderService _orderService;

        public PaymentService(IUnitOfWork<SpaPetContext> unitOfWork, ILogger<PaymentService> logger, IMapper mapper, IHttpContextAccessor httpContextAccessor, IConfiguration configuration, IOrderService orderService) : base(unitOfWork, logger, mapper, httpContextAccessor)
        {
            _configuration = configuration;
            _orderService = orderService;
        }

        public async Task<CreatePaymentResponse> ExecutePayment(CreatePaymentRequest request)
        {
            var currentTime = TimeUtils.GetCurrentSEATime();
            var currentTimeStamp = TimeUtils.GetTimestamp(currentTime);

            var txnRef = currentTime.ToString("yyMMdd") + "_" + currentTimeStamp;
            var pay = new VnPayLibrary();
            var urlCallBack = !string.IsNullOrEmpty(request.CallbackUrl) ? request.CallbackUrl : _configuration["VnPayPaymentCallBack:ReturnUrl"];

            pay.AddRequestData("vnp_Version", _configuration["Vnpay:Version"]);
            pay.AddRequestData("vnp_TmnCode", _configuration["Vnpay:TmnCode"]);
            pay.AddRequestData("vnp_Command", _configuration["Vnpay:Command"]);
            pay.AddRequestData("vnp_Amount", (((double)request.Amount) * 100).ToString());
            pay.AddRequestData("vnp_CreateDate", currentTimeStamp);
            pay.AddRequestData("vnp_CurrCode", _configuration["Vnpay:CurrCode"]);
            pay.AddRequestData("vnp_IpAddr", pay.GetIpAddress(_httpContextAccessor.HttpContext));
            pay.AddRequestData("vnp_Locale", _configuration["Vnpay:Locale"]);
            pay.AddRequestData("vnp_OrderInfo", $"Thanh toán cho đơn hàng {request.OrderId}");
            pay.AddRequestData("vnp_OrderType", "other");
            pay.AddRequestData("vnp_ReturnUrl", urlCallBack);
            pay.AddRequestData("vnp_TxnRef", txnRef);

            var paymentUrl = pay.CreateRequestUrl(_configuration["Vnpay:BaseUrl"], _configuration["Vnpay:HashSecret"]);

            var paymentId = Guid.NewGuid();
            var payment = new Payment()
            {
                Id = paymentId,
                Amount = request.Amount,
                PaymentDate = currentTime,
                PaymentMethod = request.PaymentType.GetDescriptionFromEnum(),
                OrderId = request.OrderId,
                Status = PaymentStatus.PENDING.GetDescriptionFromEnum(),
                Transactions = new List<DataTier.Models.Transaction>()
                {
                    new ()
                    {
                        Id = Guid.NewGuid(),
                        PaymentId = paymentId,
                        InvoiceCode = txnRef,
                        TotalAmount = request.Amount,
                        Description = $"Đang tiến hành thanh toán VNPAY mã đơn {txnRef}",
                        CreatedAt = currentTime,
                        PayType = request.PaymentType.GetDescriptionFromEnum(),
                        Status = PaymentStatus.PENDING.GetDescriptionFromEnum(),
                        OrderId = request.OrderId,
                        AccountId = request.AccountId
                    }
                }
            };

            var paymentResponse = new CreatePaymentResponse()
            {
                Message = "Đang tiến hành thanh toán VNPAY",
                Url = paymentUrl,
                PaymentId = payment.Id

            };



            try
            {
                //await _unitOfWork.GetRepository<TransactionPayment>().InsertAsync(transaction);
                await _unitOfWork.GetRepository<Payment>().InsertAsync(payment);
                await _unitOfWork.CommitAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            return paymentResponse;
        }

        public async Task<bool> ExecuteVnPayCallback(string? status, string? transId, string? urlCallBack)
        {
            var paymentTransaction = await _unitOfWork.GetRepository<DataTier.Models.Transaction>().SingleOrDefaultAsync(
                  predicate: x => x.InvoiceCode.Equals(transId),
                  include: x => x.Include(x => x.Payment));

            //var response = VnPayLibrary.GetFullResponseData(collections, _configuration["Vnpay:HashSecret"]);
            //var responseJson = await CallApiUtils.CallApiEndpoint(url, response);
            if (!status.Equals("00"))
            {
                paymentTransaction.Description = "VNPAY payment failed";
                paymentTransaction.Status = PaymentStatus.FAILED.GetDescriptionFromEnum();
                paymentTransaction.Payment.Status = PaymentStatus.FAILED.GetDescriptionFromEnum();
                _unitOfWork.GetRepository<DataTier.Models.Transaction>().UpdateAsync(paymentTransaction);
            }
            else
            {
                paymentTransaction.Description = "VnPay payment successful";
                paymentTransaction.Status = PaymentStatus.SUCCESS.GetDescriptionFromEnum();
                paymentTransaction.Payment.Status = PaymentStatus.SUCCESS.GetDescriptionFromEnum();
                paymentTransaction.Payment.Order.Status = OrderStatus.PAID.GetDescriptionFromEnum();
                _unitOfWork.GetRepository<DataTier.Models.Transaction>().UpdateAsync(paymentTransaction);
            }
            return await _unitOfWork.CommitAsync() > 0;
        }

        public Task<GetPaymentResponse> GetPaymentById(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<ICollection<GetPaymentResponse>> GetPaymentList(PaymentFilter filter)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> UpdatePayment(Guid id, UpdatePaymentRequest updatePaymentRequest)
        {
            string currentUser = GetUsernameFromJwt();
            var userId = await _unitOfWork.GetRepository<Account>().SingleOrDefaultAsync(
                predicate: x => x.Username.Equals(currentUser),
                selector: x => x.Id);

            var payment = await _unitOfWork.GetRepository<Payment>().SingleOrDefaultAsync(
                predicate: x => x.Id.Equals(id))
                ?? throw new BadHttpRequestException(MessageConstant.Order.OrderNotFoundMessage);

            DateTime currentTime = TimeUtils.GetCurrentSEATime();

            if (updatePaymentRequest.Status.HasValue)
            {
                payment.Status = updatePaymentRequest.Status.Value.GetDescriptionFromEnum();
            }
            else
            {
                return false;
            }

            if (!string.IsNullOrEmpty(updatePaymentRequest.Note))
            {
                payment.Note = updatePaymentRequest.Note;
            }

            var transaction = await _unitOfWork.GetRepository<DataTier.Models.Transaction>().SingleOrDefaultAsync(
                predicate: x => x.PaymentId.Equals(payment.Id))
                ?? throw new BadHttpRequestException(MessageConstant.Transaction.NotFoundFailedMessage);

            transaction.Status = updatePaymentRequest.Status.GetDescriptionFromEnum();

            _unitOfWork.GetRepository<Payment>().UpdateAsync(payment);
            _unitOfWork.GetRepository<DataTier.Models.Transaction>().UpdateAsync(transaction);

            if (updatePaymentRequest.Status.GetDescriptionFromEnum() == "SUCCESS")
            {
                Guid orderId = (Guid)payment.OrderId;

                var orderUpdateRequest = new UpdateOrderRequest
                {
                    Status = OrderStatus.PAID
                };

                bool orderUpdateSuccessful = await _orderService.UpdateOrder(orderId, orderUpdateRequest);

                if (!orderUpdateSuccessful)
                {
                    throw new Exception(MessageConstant.Transaction.UpdateTransactionFailedMessage);
                }
            }

            // Commit the changes
            bool isSuccessful = await _unitOfWork.CommitAsync() > 0;
            return isSuccessful;
        }





    }
}
