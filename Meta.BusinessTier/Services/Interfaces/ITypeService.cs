using Meta.BusinessTier.Payload;
using Meta.BusinessTier.Payload.TypePet;
using Meta.DataTier.Paginate;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

namespace Meta.BusinessTier.Services.Implements
{
    public interface ITypeService
    {
        Task<Guid> CreateNewType(CreateNewTypeRequest createNewTypeRequest);
        Task<bool> UpdateType(Guid id, UpdateTypeRequest updateTypeRequest);
        Task<IPaginate<GetTypeResponse>> GetTypes(TypesFilter filter, PagingModel pagingModel);
        Task<ICollection<GetTypeResponse>> GetTypesNoPaging(TypesFilter filter);
        Task<bool> RemoveTypeStatus(Guid id);
        Task<GetTypeResponse> GetType(Guid id);
    }
}
