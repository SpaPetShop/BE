using Meta.BusinessTier.Payload.Product;
using Meta.BusinessTier.Payload;
using Meta.DataTier.Paginate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Meta.BusinessTier.Payload.Pet;

namespace Meta.BusinessTier.Services.Interfaces
{
    public interface IPetService
    {
        Task<Guid> CreateNewPets(CreateNewPetRequest createNewPetRequest);
        Task<bool> UpdatePet(Guid id, UpdatePetRequest updatePetRequest);
        Task<IPaginate<GetPetResponse>> GetPetList(PetFilter filter, PagingModel pagingModel);
        Task<GetPetResponse> GetPetById(Guid id);
    }
}
