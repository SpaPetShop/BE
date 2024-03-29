using AutoMapper;
using Azure.Core;
using Meta.BusinessTier.Constants;
using Meta.BusinessTier.Enums;
using Meta.BusinessTier.Extensions;
using Meta.BusinessTier.Payload;
using Meta.BusinessTier.Payload.Order;
using Meta.BusinessTier.Payload.Product;
using Meta.BusinessTier.Services.Interfaces;
using Meta.BusinessTier.Utils;
using Meta.DataTier.Models;
using Meta.DataTier.Paginate;
using Meta.DataTier.Repository.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Meta.BusinessTier.Services.Implements
{
    public class OrderService : BaseService<OrderService>, IOrderService
    {
        public OrderService(IUnitOfWork<MetaContext> unitOfWork, ILogger<OrderService> logger, IMapper mapper, IHttpContextAccessor httpContextAccessor) : base(unitOfWork, logger, mapper, httpContextAccessor)
        {
        }

        public async Task<Guid> CreateNewOrder(CreateNewOrderResponse request)
        {
            DateTime currentTime = TimeUtils.GetCurrentSEATime();
            Order newOrder = new()
            {
                Id = Guid.NewGuid(),
                InvoiceCode = TimeUtils.GetTimestamp(currentTime),
                CreatedDate = currentTime,
                TotalAmount = request.TotalAmount,
                Discount = request.Discount,
                FinalAmount = request.FinalAmount,
                Note = request.Note,
                Status = OrderStatus.PENDING.GetDescriptionFromEnum(),
                UserId = request.UserId
            };

            var orderDetails = new List<OrderDetail>();
            foreach (var product in request.ProductList)
            {
                double totalProductAmount = product.SellingPrice * product.Quantity;
                orderDetails.Add(new OrderDetail
                {
                    Id = Guid.NewGuid(),
                    OrderId = newOrder.Id,
                    ProductId = product.ProductId,
                    Quantity = product.Quantity,
                    SellingPrice = product.SellingPrice,
                    TotalAmount = totalProductAmount
                });

            };
            OrderHistory history = new OrderHistory()
            {
                Id = Guid.NewGuid() ,
                Status = OrderHistoryStatus.PENDING.GetDescriptionFromEnum(),
                Note = request.Note,
                CreatedDate = currentTime,
                OrderId = newOrder.Id,
                UserId = request.UserId,
            };

            await _unitOfWork.GetRepository<Order>().InsertAsync(newOrder);
            await _unitOfWork.GetRepository<OrderDetail>().InsertRangeAsync(orderDetails);
            await _unitOfWork.GetRepository<OrderHistory>().InsertAsync(history);
            bool isSuccessful = await _unitOfWork.CommitAsync() > 0;
            if (!isSuccessful) throw new BadHttpRequestException(MessageConstant.Order.CreateOrderFailedMessage);
            return newOrder.Id;
        }

        public async Task<GetOrderDetailResponse> GetOrderDetail(Guid id)
        {
            Order order = await _unitOfWork.GetRepository<Order>().SingleOrDefaultAsync(
                predicate: x => x.Id.Equals(id))
            ?? throw new BadHttpRequestException(MessageConstant.Order.OrderNotFoundMessage);

            var orderDetailResponse = new GetOrderDetailResponse()
            {
                OrderId = order.Id,
                InvoiceCode = order.InvoiceCode,
                CreatedDate = order.CreatedDate,
                CompletedDate = order.CompletedDate,
                TotalAmount = order.TotalAmount,
                Discount = order.Discount,
                FinalAmount = order.FinalAmount,
                Note = order.Note,
                Status = EnumUtil.ParseEnum<OrderStatus>(order.Status),

                ProductList = (List<OrderDetailResponse>)await _unitOfWork.GetRepository<OrderDetail>()
                    .GetListAsync(
                        selector: x => new OrderDetailResponse()
                        {
                            OrderDetailId = x.Id,
                            ProductId = x.ProductId,
                            ProductName = x.Product.Name,
                            Quantity = x.Quantity,
                            SellingPrice = x.SellingPrice,
                            TotalAmount = x.TotalAmount
                        },
                        predicate: x => x.OrderId.Equals(id)
                    ),
                UserInfo = await _unitOfWork.GetRepository<User>().SingleOrDefaultAsync(
                    selector: x => new OrderUserResponse()
                    {
                        Id = x.Id,
                        Username = x.Username,
                        Role = EnumUtil.ParseEnum<RoleEnum>(x.Role)
                    },
                    predicate: x => x.Id.Equals(order.UserId))
                
            };
            return orderDetailResponse;
        }
        private static Expression<Func<Order, bool>> BuildGetOrderQuery(OrderFilter filter)
        {
            Expression<Func<Order, bool>> filterQuery = x => true;

            var InvoiceCode = filter.InvoiceCode;
            var fromDate = filter.fromDate;
            var toDate = filter.toDate;
            var status = filter.status;

            if (InvoiceCode != null)
            {
                filterQuery = filterQuery.AndAlso(x => x.InvoiceCode.Contains(InvoiceCode));
            }

            if (fromDate.HasValue)
            {
                filterQuery = filterQuery.AndAlso(x => x.CreatedDate >= fromDate);
            }

            if (toDate.HasValue)
            {
                filterQuery = filterQuery.AndAlso(x => x.CreatedDate <= toDate);
            }

            if (status != null)
            {
                filterQuery = filterQuery.AndAlso(x => x.Status.Equals(status.GetDescriptionFromEnum()));
            }


            return filterQuery;
        }

        public async Task<IPaginate<GetOrderResponse>> GetOrderList(OrderFilter filter, PagingModel pagingModel)
        {
            IPaginate<GetOrderResponse> response = await _unitOfWork.GetRepository<Order>().GetPagingListAsync(
                selector: x => _mapper.Map<GetOrderResponse>(x),
                predicate: BuildGetOrderQuery(filter),
                orderBy: x => x.OrderByDescending(x => x.CreatedDate),
                page: pagingModel.page,
                size: pagingModel.size
                );
            return response;
        }

        public async Task<bool> UpdateOrder(Guid orderId, UpdateOrderRequest request)
        {
            string currentUser = GetUsernameFromJwt();
            var userId = await _unitOfWork.GetRepository<User>().SingleOrDefaultAsync(
                predicate : x => x.Username.Equals(currentUser),
                selector: x => x.Id); 
            Order updateOrder = await _unitOfWork.GetRepository<Order>().SingleOrDefaultAsync(
                predicate: x => x.Id.Equals(orderId))
                ?? throw new BadHttpRequestException(MessageConstant.Order.OrderNotFoundMessage);
            DateTime currentTime = TimeUtils.GetCurrentSEATime();
            switch (request.Status)
            {
                case OrderStatus.COMPLETED:
                    break;
                case OrderStatus.CONFIRMED:
                    break;
                case OrderStatus.PAID:
                    break;
                case OrderStatus.CANCELED:
                    updateOrder.Status = OrderStatus.CANCELED.GetDescriptionFromEnum();
                    _unitOfWork.GetRepository<Order>().UpdateAsync(updateOrder);
                    
                    break;
                default:
                    return false;
            }
            OrderHistory history = new OrderHistory()
            {
                Id = Guid.NewGuid(),
                Status = request.Status.GetDescriptionFromEnum(),
                Note = request.Note,
                CreatedDate = currentTime,
                OrderId = orderId,
                UserId = userId,
            };
            await _unitOfWork.GetRepository<OrderHistory>().InsertAsync(history);
            bool isSuccessful = await _unitOfWork.CommitAsync() > 0;
            return isSuccessful;
        }

        public async Task<IEnumerable<GetOrderHistoriesResponse>> GetOrderHistories(Guid orderId)
        {
            IEnumerable<GetOrderHistoriesResponse> respone = await _unitOfWork.GetRepository<OrderHistory>().GetListAsync(
               selector: x => _mapper.Map<GetOrderHistoriesResponse>(x),
               include: x => x.Include(x => x.User),
               orderBy: x => x.OrderByDescending(x => x.CreatedDate));
            return respone;
        }
    }
}

