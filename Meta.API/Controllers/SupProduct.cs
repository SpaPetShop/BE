using DentalLabManagement.API.Controllers;
using Meta.BusinessTier.Constants;
using Meta.BusinessTier.Payload.Product;
using Meta.BusinessTier.Payload;
using Meta.BusinessTier.Services.Implements;
using Meta.BusinessTier.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Meta.BusinessTier.Payload.PetService;

namespace Meta.API.Controllers
{
    [ApiController]
    public class SupProduct : BaseController<SupProduct>
    {
        readonly ISupProductService _supProductService;

        public SupProduct(ILogger<SupProduct> logger, ISupProductService supProductService) : base(logger)
        {
            _supProductService = supProductService;
        }
        [HttpPost(ApiEndPointConstant.SupProduct.SupProductsEndPoint)]
        public async Task<IActionResult> CreateNewPSupProduct(CreateNewSupProductRequest product)
        {
            var response = await _supProductService.CreateNewPSupProduct(product);
            return Ok(response);
        }
        [HttpGet(ApiEndPointConstant.SupProduct.SupProductsEndPoint)]
        public async Task<IActionResult> GetSupProductList([FromQuery] SupProductFilter filter, [FromQuery] PagingModel pagingModel)
        {
            var response = await _supProductService.GetSupProductList(filter, pagingModel);
            return Ok(response);
        }

        [HttpGet(ApiEndPointConstant.SupProduct.SupProductEndPoint)]
        public async Task<IActionResult> GetSupProductById(Guid id)
        {
            var response = await _supProductService.GetSupProductById(id);
            return Ok(response);
        }
        [HttpPut(ApiEndPointConstant.SupProduct.SupProductsEndPoint)]
        public async Task<IActionResult> UpdateSupProduct(Guid id, UpdateSupProductRequest updateProductRequest)
        {
            var response = await _supProductService.UpdateSupProduct(id, updateProductRequest);
            if (!response) return Ok(MessageConstant.Product.UpdateProductFailedMessage);
            return Ok(MessageConstant.Product.UpdateStatusSuccessMessage);
        }
        [HttpDelete(ApiEndPointConstant.SupProduct.SupProductEndPoint)]
        public async Task<IActionResult> RemoveSupProductStatus(Guid id)
        {
            var response = await _supProductService.RemoveSupProductStatus(id);
            if (!response) return Ok(MessageConstant.Product.UpdateProductFailedMessage);
            return Ok(MessageConstant.Product.UpdateStatusSuccessMessage);

        }
    }
}
