using AutoMapper;
using Meta.BusinessTier.Constants;
using Meta.BusinessTier.Payload.Task;
using Meta.BusinessTier.Services.Interfaces;
using Meta.DataTier.Models;
using Meta.DataTier.Repository.Interfaces;
using Microsoft.AspNetCore.Http;
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
        public TaskService(IUnitOfWork<MetaContext> unitOfWork, ILogger<TaskService> logger, IMapper mapper, IHttpContextAccessor httpContextAccessor) : base(unitOfWork, logger, mapper, httpContextAccessor)
        {
        }

        public Task<Guid> CreateNewTask(CreateNewTaskRequest createNewTaskRequest)
        {
            throw new NotImplementedException();
        }

        public Task<GetTaskResponse> GetTaskById(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<ICollection<GetTaskResponse>> GetTaskList(TaskFilter filter)
        {
            throw new NotImplementedException();
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


        public Task<bool> UpdateTask(Guid id, UpdateTaskRequest updateTaskRequest)
        {
            throw new NotImplementedException();
        }
    }
}
