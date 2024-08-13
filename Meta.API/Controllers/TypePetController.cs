using DentalLabManagement.API.Controllers;
using Meta.BusinessTier.Constants;
using Meta.BusinessTier.Payload.Category;
using Meta.BusinessTier.Payload;
using Meta.BusinessTier.Services.Implements;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Meta.BusinessTier.Payload.TypePet;

namespace Meta.API.Controllers
{

    [ApiController]
    public class TypePetController : BaseController<TypePetController>
    {
        readonly ITypeService _typeService;

        public TypePetController(ILogger<TypePetController> logger, ITypeService typeService) : base(logger)
        {
            _typeService = typeService;
        }
        [HttpPost(ApiEndPointConstant.TypePet.TypePetsEndPoint)]
        public async Task<IActionResult> CreateNewType(CreateNewTypeRequest request)
        {
            var response = await _typeService.CreateNewType(request);
            return Ok(response);
        }
        [HttpGet(ApiEndPointConstant.TypePet.TypePetsEndPoint)]
        public async Task<IActionResult> GetTypes([FromQuery] TypesFilter filter, [FromQuery] PagingModel pagingModel)
        {
            var response = await _typeService.GetTypes(filter, pagingModel);
            return Ok(response);
        }
        [HttpGet(ApiEndPointConstant.TypePet.TypePetsNopagingNateEndPoint)]
        public async Task<IActionResult> GetTypesNoPaging([FromQuery] TypesFilter filter)
        {
            var response = await _typeService.GetTypesNoPaging(filter);
            return Ok(response);
        }
        [HttpGet(ApiEndPointConstant.TypePet.TypePetEndPoint)]
        public async Task<IActionResult> GetType(Guid id)
        {
            var response = await _typeService.GetType(id);
            return Ok(response);
        }
        [HttpPut(ApiEndPointConstant.TypePet.TypePetEndPoint)]
        public async Task<IActionResult> UpdateType(Guid id, UpdateTypeRequest updateCategoryRequest)
        {
            var isSuccessful = await _typeService.UpdateType(id, updateCategoryRequest);
            if (!isSuccessful) return Ok(MessageConstant.TypePet.UpdateTypeFailedMessage);
            return Ok(MessageConstant.TypePet.UpdateTypeSuccessMessage);
        }
        [HttpDelete(ApiEndPointConstant.TypePet.TypePetEndPoint)]
        public async Task<IActionResult> RemoveTypeStatus(Guid id)
        {
            var isSuccessful = await _typeService.RemoveTypeStatus(id);
            if (!isSuccessful) return Ok(MessageConstant.TypePet.UpdateTypeFailedMessage);
            return Ok(MessageConstant.TypePet.UpdateTypeSuccessMessage);
        }
    }
}
