using DentalLabManagement.API.Controllers;
using Meta.BusinessTier.Constants;
using Meta.BusinessTier.Payload.Product;
using Meta.BusinessTier.Payload;
using Meta.BusinessTier.Services.Implements;
using Meta.BusinessTier.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Meta.BusinessTier.Payload.Pet;
using Microsoft.AspNetCore.Authorization;
using HiCamping.BusinessTier.Validators;
using Meta.BusinessTier.Enums.Status;

namespace Meta.API.Controllers
{
    [ApiController]
    public class PetController : BaseController<PetController>
    {
        readonly IPetService _petService;

        public PetController(ILogger<PetController> logger, IPetService petService) : base(logger)
        {
            _petService = petService;
        }
        [CustomAuthorize(RoleEnum.User)]
        [HttpPost(ApiEndPointConstant.Pet.PetsEndPoint)]
        public async Task<IActionResult> CreateNewPets(CreateNewPetRequest pet)
        {
            var response = await _petService.CreateNewPets(pet);
            return Ok(response);
        }
        [HttpGet(ApiEndPointConstant.Pet.PetsEndPoint)]
        public async Task<IActionResult> GetPetList([FromQuery] PetFilter filter, [FromQuery] PagingModel pagingModel)
        {
            var response = await _petService.GetPetList(filter, pagingModel);
            return Ok(response);
        }
        [HttpGet(ApiEndPointConstant.Pet.PetEndPoint)]
        public async Task<IActionResult> GetPetById(Guid id)
        {
            var response = await _petService.GetPetById(id);
            return Ok(response);
        }
        [HttpPut(ApiEndPointConstant.Pet.PetEndPoint)]
        public async Task<IActionResult> UpdatePet(Guid id, UpdatePetRequest updatePetRequest)
        {
            var response = await _petService.UpdatePet(id, updatePetRequest);
            if (!response) return Ok(MessageConstant.Product.UpdateProductFailedMessage);
            return Ok(MessageConstant.Product.UpdateStatusSuccessMessage);
        }
    }
}