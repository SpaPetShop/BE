using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Meta.BusinessTier.Payload;
using Meta.BusinessTier.Payload.PetService;
using Meta.DataTier.Paginate;

namespace Meta.BusinessTier.Services.Interfaces
{
    public interface ISupProductService
    {
        Task<Guid> CreateNewPSupProduct(CreateNewSupProductRequest createNewPetServiceRequest);
        Task<bool> UpdateSupProduct(Guid id, UpdateSupProductRequest updatePetServiceRequest);
        Task<IPaginate<GetSupProductsResponse>> GetSupProductList(SupProductFilter filter, PagingModel pagingModel);
        Task<GetSupProductsResponse> GetSupProductById(Guid id);
        Task<bool> RemoveSupProductStatus(Guid id);
    }
}
