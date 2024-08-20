using DentalLabManagement.API.Controllers;
using Meta.BusinessTier.Constants;
using Meta.BusinessTier.Payload.Category;
using Meta.BusinessTier.Payload;
using Meta.BusinessTier.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Meta.BusinessTier.Payload.CustomerRequest;
using HiCamping.BusinessTier.Validators;
using Meta.BusinessTier.Enums.Status;

namespace Meta.API.Controllers
{
    [ApiController]
    public class CustomerRequestController : BaseController<CustomerRequestController>
    {
        readonly ICustomerRequestService _customerRequestService;

        public CustomerRequestController(ILogger<CustomerRequestController> logger, ICustomerRequestService customerRequestService) : base(logger)
        {
            _customerRequestService = customerRequestService;
        }
        [CustomAuthorize(RoleEnum.USER)]
        [HttpPost(ApiEndPointConstant.CustomerRequest.CustomerRequestsEndPoint)]
        public async Task<IActionResult> CreateNewCustomerRequest(CreateNewCustomerRequest createNewCustomerRequest)
        {
            var response = await _customerRequestService.CreateNewCustomerRequest(createNewCustomerRequest);
            return Ok(response);
        }
        [HttpGet(ApiEndPointConstant.CustomerRequest.CustomerRequestsEndPoint)]
        public async Task<IActionResult> GetCustomerRequestList([FromQuery] CustomerRequestFilter filter, [FromQuery] PagingModel pagingModel)
        {
            var response = await _customerRequestService.GetCustomerRequestList(filter, pagingModel);
            return Ok(response);
        }

        [HttpGet(ApiEndPointConstant.CustomerRequest.CustomerRequestEndPoint)]
        public async Task<IActionResult> GetCustomerRequestById(Guid id)
        {
            var response = await _customerRequestService.GetCustomerRequestById(id);
            return Ok(response);
        }
        [HttpPut(ApiEndPointConstant.CustomerRequest.CustomerRequestEndPoint)]
        public async Task<IActionResult> UpdateCustomerRequest(Guid id, UpdateCustomerRequest updateCustomerRequest)
        {
            var isSuccessful = await _customerRequestService.UpdateCustomerRequest(id, updateCustomerRequest);
            if (!isSuccessful) return Ok(MessageConstant.CustomerRequest.UpdateFailedMessage);
            return Ok(MessageConstant.CustomerRequest.UpdateSuccessMessage);
        }
    }
}
