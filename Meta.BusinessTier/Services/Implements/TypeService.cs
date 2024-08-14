using AutoMapper;
using Meta.BusinessTier.Constants;
using Meta.BusinessTier.Enums.Status;
using Meta.BusinessTier.Enums.Type;
using Meta.BusinessTier.Payload;
using Meta.BusinessTier.Payload.TypePet;
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
using System.Reflection;
using System.Threading.Tasks;

namespace Meta.BusinessTier.Services.Implements
{
    public class TypeService : BaseService<TypeService>, ITypeService
    {
        public TypeService(IUnitOfWork<MetaContext> unitOfWork, ILogger<TypeService> logger, IMapper mapper, IHttpContextAccessor httpContextAccessor) : base(unitOfWork, logger, mapper, httpContextAccessor)
        {
        }


        public async Task<GetTypeResponse> GetType(Guid id)
        {
            if (id == Guid.Empty) throw new BadHttpRequestException(MessageConstant.TypePet.TypeEmptyMessage);
            TypePet type = await _unitOfWork.GetRepository<TypePet>().SingleOrDefaultAsync(
                predicate: x => x.Id.Equals(id))
                ?? throw new BadHttpRequestException(MessageConstant.TypePet.NotFoundFailedMessage);
            return _mapper.Map<GetTypeResponse>(type);
        }


        public async Task<Guid> CreateNewType(CreateNewTypeRequest createNewTypeRequest)
        {
            TypePet newType = _mapper.Map<TypePet>(createNewTypeRequest);
            newType.Id = Guid.NewGuid();
            newType.Status = TypePetStatus.Active.GetDescriptionFromEnum();

            await _unitOfWork.GetRepository<TypePet>().InsertAsync(newType);

            bool isSuccessful = await _unitOfWork.CommitAsync() > 0;
            if (!isSuccessful) throw new BadHttpRequestException(MessageConstant.TypePet.CreateTypeFailedMessage);
            return newType.Id;
        }



        public async Task<IPaginate<GetTypeResponse>> GetTypes(TypesFilter filter, PagingModel pagingModel)
        {
            IPaginate<GetTypeResponse> response = await _unitOfWork.GetRepository<TypePet>().GetPagingListAsync(
               selector: x => _mapper.Map<GetTypeResponse>(x),
               filter: filter,
               page: pagingModel.page,
               size: pagingModel.size);
            return response;
        }

        public async Task<ICollection<GetTypeResponse>> GetTypesNoPaging(TypesFilter filter)
        {
            ICollection<GetTypeResponse> response = await _unitOfWork.GetRepository<TypePet>().GetListAsync(
               selector: x => _mapper.Map<GetTypeResponse>(x),
               filter: filter);
            return response;
        }
        public async Task<bool> UpdateType(Guid id, UpdateTypeRequest updateTypeRequest)
        {
            if (id == Guid.Empty) throw new BadHttpRequestException(MessageConstant.TypePet.TypeEmptyMessage);

            TypePet type = await _unitOfWork.GetRepository<TypePet>().SingleOrDefaultAsync(
                predicate: x => x.Id.Equals(id),
                include: x => x.Include(x => x.Pets))
                ?? throw new BadHttpRequestException(MessageConstant.TypePet.NotFoundFailedMessage);

            // Update the name and description
            type.Name = string.IsNullOrEmpty(updateTypeRequest.Name) ? type.Name : updateTypeRequest.Name;
            type.Description = string.IsNullOrEmpty(updateTypeRequest.Description) ? type.Description : updateTypeRequest.Description;

            // Update the status if provided
            if (updateTypeRequest.Status != null)
            {
                type.Status = updateTypeRequest.Status.GetDescriptionFromEnum();
            }
            else
            {
                throw new BadHttpRequestException("Cần nhập trạng thái của loại thú cưng vào");
            }

            _unitOfWork.GetRepository<TypePet>().UpdateAsync(type);
            bool isSuccess = await _unitOfWork.CommitAsync() > 0;
            return isSuccess;
        }

        public Task<bool> RemoveTypeStatus(Guid id)
        {
            throw new NotImplementedException();
        }
    }
}
