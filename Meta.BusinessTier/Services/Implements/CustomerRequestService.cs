using AutoMapper;
using Meta.BusinessTier.Constants;
using Meta.BusinessTier.Enums.EnumStatus;
using Meta.BusinessTier.Enums.Status;
using Meta.BusinessTier.Payload;
using Meta.BusinessTier.Payload.CustomerRequest;
using Meta.BusinessTier.Payload.Pet;
using Meta.BusinessTier.Payload.Product;
using Meta.BusinessTier.Payload.Task;
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
using System.Text;
using System.Threading.Tasks;

namespace Meta.BusinessTier.Services.Implements
{
    public class CustomerRequestService : BaseService<CustomerRequestService>, ICustomerRequestService
    {
        public CustomerRequestService(IUnitOfWork<MetaContext> unitOfWork, ILogger<CustomerRequestService> logger, IMapper mapper, IHttpContextAccessor httpContextAccessor) : base(unitOfWork, logger, mapper, httpContextAccessor)
        {
        }

        public async Task<Guid> CreateNewCustomerRequest(CreateNewCustomerRequest createNewCustomerRequest)
        {
            DateTime currentTime = TimeUtils.GetCurrentSEATime();
            var currentUser = GetUsernameFromJwt();
            var user = await _unitOfWork.GetRepository<Account>().SingleOrDefaultAsync(
                predicate: x => x.Username.Equals(currentUser))
                ?? throw new BadHttpRequestException(MessageConstant.Account.NotFoundFailedMessage);

            var newRequest = new CustomerRequest
            {
                Id = Guid.NewGuid(),
                ExctionDate = createNewCustomerRequest.ExctionDate,
                Note = createNewCustomerRequest.Note,
                Status = CustomerRequestStatus.PENDING.GetDescriptionFromEnum(),
                OrderId = createNewCustomerRequest.OrderId,
                StaffId = createNewCustomerRequest.StaffId,
                UserId = user.Id,
                CreateDate = currentTime
            };
            await _unitOfWork.GetRepository<CustomerRequest>().InsertAsync(newRequest);
            bool isSuccess = await _unitOfWork.CommitAsync() > 0;
            return newRequest.Id;
        }

        public async Task<GetCustomerRequestResponse> GetCustomerRequestById(Guid id)
        {
            var customerRequest = await _unitOfWork.GetRepository<CustomerRequest>().SingleOrDefaultAsync(
                predicate: x => x.Id.Equals(id),
                selector: x => new GetCustomerRequestResponse
                {
                    Id = x.Id,
                    Note = x.Note,
                    Status = x.Status,
                    CreateDate = x.CreateDate,
                    ExctionDate = x.ExctionDate,
                    //StaffId = new AccountResponse
                    //{
                    //    Id = x.Staff.Id,
                    //    FullName = x.Staff.FullName,
                    //    Role = EnumUtil.ParseEnum<RoleEnum>(x.Staff.Role)
                    //},
                    //UserId = new AccountResponse
                    //{
                    //    Id = x.User.Id,
                    //    FullName = x.User.FullName,
                    //    Role = EnumUtil.ParseEnum<RoleEnum>(x.User.Role)
                    //},
                    OrderId = x.OrderId
                },
                include: x => x.Include(cr => cr.Order)) 
            ?? throw new BadHttpRequestException("Customer request not found.");


            return customerRequest;
        }


        public async Task<IPaginate<GetCustomerRequestResponse>> GetCustomerRequestList(CustomerRequestFilter filter, PagingModel pagingModel)
        {
            IPaginate<GetCustomerRequestResponse> customerRequestList = await _unitOfWork.GetRepository<CustomerRequest>().GetPagingListAsync(
                selector: x => new GetCustomerRequestResponse
                {
                    Id = x.Id,
                    Note = x.Note,
                    Status = x.Status,
                    CreateDate = x.CreateDate,
                    ExctionDate = x.ExctionDate,
                    OrderId = x.OrderId,
                    //StaffId = new AccountResponse
                    //{
                    //    Id = x.Staff.Id,
                    //    FullName = x.Staff.FullName,
                    //    Role = EnumUtil.ParseEnum<RoleEnum>(x.Staff.Role)
                    //},
                    //UserId = new AccountResponse
                    //{
                    //    Id = x.User.Id,
                    //    FullName = x.User.FullName,
                    //    Role = EnumUtil.ParseEnum<RoleEnum>(x.User.Role)
                    //}
                },
                filter: filter,
                orderBy: x => x.OrderByDescending(x => x.CreateDate),
                include: x => x.Include(cr => cr.Order),
                page: pagingModel.page,
                size: pagingModel.size
            );

            return customerRequestList;
        }


        public async Task<bool> UpdateCustomerRequest(Guid id, UpdateCustomerRequest updateCustomerRequest)
        {
            DateTime currentTime = TimeUtils.GetCurrentSEATime();

            var request = await _unitOfWork.GetRepository<CustomerRequest>().SingleOrDefaultAsync(
                predicate: x => x.Id.Equals(id))
                ?? throw new BadHttpRequestException(MessageConstant.TaskManager.TaskNotFoundMessage);

            var order = await _unitOfWork.GetRepository<Order>().SingleOrDefaultAsync(
                predicate: x => x.Id.Equals(request.OrderId))
                ?? throw new BadHttpRequestException(MessageConstant.Order.OrderNotFoundMessage);

            var task = await _unitOfWork.GetRepository<TaskManager>().SingleOrDefaultAsync(
                predicate: x => x.OrderId.Equals(order.Id))
                ?? throw new BadHttpRequestException(MessageConstant.TaskManager.TaskNotFoundMessage);

            var staff = await _unitOfWork.GetRepository<Account>().SingleOrDefaultAsync(
                predicate: x => x.Id.Equals(task.AccountId))
                ?? throw new BadHttpRequestException(MessageConstant.Account.NotFoundFailedMessage);

            if (updateCustomerRequest.Status == CustomerRequestStatus.ACCEPT && updateCustomerRequest.Status == CustomerRequestStatus.REJECT)
            {
                throw new BadHttpRequestException(MessageConstant.CustomerRequest.UpdateStatusCompletedFailedMessage);
            }
            switch (updateCustomerRequest.Status)
            {
                case CustomerRequestStatus.ACCEPT:
                    updateCustomerRequest.Status = CustomerRequestStatus.ACCEPT;
                    if(request.ExctionDate != null)
                    {
                        order.ExcutionDate = request.ExctionDate;
                        task.ExcutionDate = request.ExctionDate;
                        _unitOfWork.GetRepository<TaskManager>().UpdateAsync(task);
                        _unitOfWork.GetRepository<Order>().UpdateAsync(order);
                    }
                    if(request.StaffId != null)
                    {
                        task.AccountId = request.StaffId;
                        _unitOfWork.GetRepository<TaskManager>().UpdateAsync(task);
                    }
                    var note = new Note()
                    {
                        Id = Guid.NewGuid(),
                        Status = NoteStatus.SUCCESS.GetDescriptionFromEnum(),
                        CreateDate = currentTime,
                        Description = updateCustomerRequest.Note,
                        OrderId = order.Id

                    };
                    if (note != null)
                    {
                        await _unitOfWork.GetRepository<Note>().InsertAsync(note);
                    }

                    break;
                case CustomerRequestStatus.REJECT:
                    updateCustomerRequest.Status = CustomerRequestStatus.REJECT;
                    note = new Note()
                    {
                        Id = Guid.NewGuid(),
                        Status = NoteStatus.FAILED.GetDescriptionFromEnum(),
                        CreateDate = currentTime,
                        Description = updateCustomerRequest.Note,
                        OrderId = order.Id

                    };
                    if (note != null)
                    {
                        await _unitOfWork.GetRepository<Note>().InsertAsync(note);
                    }
                    break;
                default:
                    return false;
            }
            return true;
        }
    }
}
