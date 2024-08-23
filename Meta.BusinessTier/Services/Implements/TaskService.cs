using AutoMapper;
using Azure.Core;
using Meta.BusinessTier.Constants;
using Meta.BusinessTier.Enums.EnumStatus;
using Meta.BusinessTier.Enums.EnumTypes;
using Meta.BusinessTier.Enums.Status;
using Meta.BusinessTier.Enums.Type;
using Meta.BusinessTier.Payload.Order;
using Meta.BusinessTier.Payload.Task;
using Meta.BusinessTier.Services.Interfaces;
using Meta.BusinessTier.Utils;
using Meta.DataTier.Models;
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
    public class TaskService : BaseService<TaskService>, ITaskService
    {
        public TaskService(IUnitOfWork<SpaPetContext> unitOfWork, ILogger<TaskService> logger, IMapper mapper, IHttpContextAccessor httpContextAccessor) : base(unitOfWork, logger, mapper, httpContextAccessor)
        {
        }
        public async Task<Guid> CreateNewTask(CreateNewTaskRequest createNewTaskRequest)
        {
            var currentUser = GetUsernameFromJwt();
            Account account = await _unitOfWork.GetRepository<Account>().SingleOrDefaultAsync(
                predicate: x => x.Username.Equals(currentUser));

            DateTime currentTime = TimeUtils.GetCurrentSEATime();

            var tasksForToday = await _unitOfWork.GetRepository<TaskManager>().GetListAsync(
                predicate: t => t.AccountId == createNewTaskRequest.AccountId
                                && t.ExcutionDate.HasValue
                                && t.ExcutionDate.Value.Date == currentTime.Date);

            int taskCountForToday = tasksForToday.Count();


            var processTasks = await _unitOfWork.GetRepository<TaskManager>().GetListAsync(
                predicate: t => t.AccountId == createNewTaskRequest.AccountId
                                && t.Status == TaskManagerStatus.PENDING.GetDescriptionFromEnum() && t.ExcutionDate.HasValue && t.ExcutionDate.Value.Day == currentTime.Day);

            int processTaskCount = processTasks.Count();

            if (taskCountForToday >= 4)
            {
                throw new BadHttpRequestException(MessageConstant.TaskManager.FullTaskMessage);
            }

            // Kiểm tra xem có task nào chưa hoàn thành cho đơn hàng này không
            var existingTasksForOrder = await _unitOfWork.GetRepository<TaskManager>().GetListAsync(
                predicate: t => t.OrderId == createNewTaskRequest.OrderId
                                && t.Status != TaskManagerStatus.COMPLETED.GetDescriptionFromEnum());

            if (existingTasksForOrder.Any())
            {
                throw new BadHttpRequestException(MessageConstant.TaskManager.ExceedQuantityMessage);
            }

            var order = await _unitOfWork.GetRepository<Order>().SingleOrDefaultAsync(
                predicate: o => o.Id == createNewTaskRequest.OrderId
                                && o.Type == OrderType.MANAGERREQUEST.GetDescriptionFromEnum())
                    ?? throw new BadHttpRequestException(MessageConstant.Order.OrderNoteManagerRequestMessage);

            TaskManager newTask = new()
            {
                Id = Guid.NewGuid(),
                Type = TaskType.MANAGERREQUEST.GetDescriptionFromEnum(),
                Status = TaskManagerStatus.PROCESS.GetDescriptionFromEnum(),
                CreateDate = currentTime,
                ExcutionDate = order.ExcutionDate,
                EstimatedCompletionDate = order.EstimatedCompletionDate,
                AccountId = createNewTaskRequest.AccountId,
                OrderId = createNewTaskRequest.OrderId,
            };

            await _unitOfWork.GetRepository<TaskManager>().InsertAsync(newTask);
            bool isSuccessful = await _unitOfWork.CommitAsync() > 0;

            if (!isSuccessful)
            {
                throw new BadHttpRequestException(MessageConstant.TaskManager.CreateNewTaskFailedMessage);
            }

            return newTask.Id;
        }


        public async Task<GetTaskResponse> GetTaskById(Guid id)
        {
            var task = await _unitOfWork.GetRepository<TaskManager>()
                .SingleOrDefaultAsync(
                    predicate: x => x.Id.Equals(id),
                    selector: task => new GetTaskResponse
                    {
                        Id = task.Id,
                        Type = EnumUtil.ParseEnum<TaskType>(task.Type),
                        CreateDate = task.CreateDate,
                        Status = EnumUtil.ParseEnum<TaskManagerStatus>(task.Status),
                        CompletedDate = task.CompletedDate,
                        ExcutionDate = task.ExcutionDate,
                        EstimatedCompletionDate = task.EstimatedCompletionDate,
                        Order = new OrderResponse
                        {
                            Id = task.Order.Id,
                            InvoiceCode = task.Order.InvoiceCode,
                            FinalAmount = task.Order.FinalAmount
                        },
                        Staff = task.Account == null ? null : new AccountResponse
                        {
                            Id = task.Account.Id,
                            FullName = task.Account.FullName,
                            Role = EnumUtil.ParseEnum<RoleEnum>(task.Account.Role),
                        },
                        Pets = task.Order.Pet == null ? null : new OrderPetResponse
                        {
                            Id = task.Order.Pet.Id,
                            Name = task.Order.Pet.Name,
                        },

                    },
                    orderBy: null, // Replace with your desired order by expression if needed
                    include: x => x.Include(x => x.Order)
                                       .ThenInclude(o => o.Pet.Account)
                                       .ThenInclude(o => o.Pets));

            return task;
        }

        public async Task<ICollection<GetTaskResponse>> GetTaskList(TaskFilter filter)
        {
            var taskList = await _unitOfWork.GetRepository<TaskManager>()
                .GetListAsync(
                    selector: task => new GetTaskResponse
                    {
                        Id = task.Id,
                        Type = EnumUtil.ParseEnum<TaskType>(task.Type),
                        CreateDate = task.CreateDate,
                        Status = EnumUtil.ParseEnum<TaskManagerStatus>(task.Status),
                        CompletedDate = task.CompletedDate,
                        ExcutionDate = task.ExcutionDate,
                        EstimatedCompletionDate = task.EstimatedCompletionDate,
                        Order = new OrderResponse
                        {
                            Id = task.Order.Id,
                            InvoiceCode = task.Order.InvoiceCode,
                            FinalAmount = task.Order.FinalAmount
                        },
                        Staff = task.Account == null ? null : new AccountResponse
                        {
                            Id = task.Account.Id,
                            FullName = task.Account.FullName,
                            Role = EnumUtil.ParseEnum<RoleEnum>(task.Account.Role),
                        },
                        Pets = task.Order.Pet == null ? null : new OrderPetResponse
                        {
                            Id = task.Order.Pet.Id,
                            Name = task.Order.Pet.Name,
                        },
                    },
                    filter: filter,
                    orderBy: null, // Replace with your desired order by expression if needed
                    include: x => x.Include(x => x.Order)
                                       .ThenInclude(o => o.Pet.Account)
                                       .ThenInclude(o => o.Pets)

                );

            return taskList;
        }
        public async Task<bool> DeletaTask(Guid id)
        {
            var task = await _unitOfWork.GetRepository<TaskManager>()
                .SingleOrDefaultAsync(predicate: x => x.Id.Equals(id));

            if (task == null)
            {
                throw new BadHttpRequestException(MessageConstant.TaskManager.TaskNotFoundMessage);
            }

            _unitOfWork.GetRepository<TaskManager>().DeleteAsync(task);
            await _unitOfWork.CommitAsync();

            return true;
        }


        public async Task<bool> UpdateTask(Guid id, UpdateTaskRequest updateTaskRequest)
        {
            if (id == Guid.Empty)
                throw new BadHttpRequestException(MessageConstant.TaskManager.EmptyTaskIdMessage);

            TaskManager task = await _unitOfWork.GetRepository<TaskManager>().SingleOrDefaultAsync(
                predicate: x => x.Id.Equals(id))
                ?? throw new BadHttpRequestException(MessageConstant.TaskManager.TaskNotFoundMessage);

            Account account = await _unitOfWork.GetRepository<Account>().SingleOrDefaultAsync(
                predicate: x => x.Id.Equals(updateTaskRequest.AccountId))
                ?? throw new BadHttpRequestException(MessageConstant.Account.NotFoundFailedMessage);

            task.AccountId = updateTaskRequest.AccountId;
            task.ExcutionDate = updateTaskRequest.ExcutionDate.HasValue ? updateTaskRequest.ExcutionDate : task.ExcutionDate;
            task.Status = updateTaskRequest.Status.GetDescriptionFromEnum();

            // Cập nhật Order
            var order = await _unitOfWork.GetRepository<Order>().SingleOrDefaultAsync(
                predicate: o => o.Id == task.OrderId);

            if (order == null)
                throw new BadHttpRequestException(MessageConstant.Order.OrderNotFoundMessage);

            order.ExcutionDate = updateTaskRequest.ExcutionDate.HasValue ? updateTaskRequest.ExcutionDate : order.ExcutionDate;

            // Tính toán lại EstimatedCompletionDate dựa trên TimeWork trong Order
            if (order.TimeWork.HasValue)
            {
                order.EstimatedCompletionDate = order.ExcutionDate?.AddHours(order.TimeWork.Value);
                task.EstimatedCompletionDate = order.EstimatedCompletionDate;
            }

            _unitOfWork.GetRepository<TaskManager>().UpdateAsync(task);
            _unitOfWork.GetRepository<Order>().UpdateAsync(order);
            bool isSuccess = await _unitOfWork.CommitAsync() > 0;
            return isSuccess;
        }

    }
}
