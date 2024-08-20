using Meta.BusinessTier.Payload.Pet;
using Meta.BusinessTier.Payload;
using Meta.DataTier.Paginate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Meta.BusinessTier.Payload.CustomerRequest;

namespace Meta.BusinessTier.Services.Interfaces
{
    public interface ICustomerRequestService
    {
        Task<Guid> CreateNewCustomerRequest(CreateNewCustomerRequest createNewCustomerRequest);
        Task<IPaginate<GetCustomerRequestResponse>> GetCustomerRequestList(CustomerRequestFilter filter, PagingModel pagingModel);
        Task<GetCustomerRequestResponse> GetCustomerRequestById(Guid id);
        Task<bool> UpdateCustomerRequest(Guid id,UpdateCustomerRequest updateCustomerRequest);
    }
}
