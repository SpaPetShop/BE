using DentalLabManagement.API.Controllers;
using Meta.BusinessTier.Constants;
using Meta.BusinessTier.Payload;
using Meta.BusinessTier.Payload.Category;
using Meta.BusinessTier.Payload.Product;
using Meta.BusinessTier.Services.Implements;
using Meta.BusinessTier.Services.Interfaces;
using Meta.DataTier.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Meta.API.Controllers
{
    [ApiController]
    public class ProductController : BaseController<ProductController>
    {
        private readonly IProductService _iProductService;

        public ProductController(ILogger<ProductController> logger, IProductService productService) : base(logger)
        {
            _iProductService = productService;
        }
        [HttpPost(ApiEndPointConstant.Product.ProductsEndPoint)]
        public async Task<IActionResult> CreateNewProducts(CreateNewProductRequest product)
        {
            var response = await _iProductService.CreateNewProducts(product);
            return Ok(response);
        }
        [HttpGet(ApiEndPointConstant.Product.ProductsEndPoint)]
        public async Task<IActionResult> GetProductList([FromQuery] ProductFilter filter, [FromQuery] PagingModel pagingModel)
        {
            var response = await _iProductService.GetProductList(filter, pagingModel);
            return Ok(response);
        }
        [HttpGet(ApiEndPointConstant.Product.ProductEndPoint)]
        public async Task<IActionResult> GetProductById(Guid id)
        {
            var response = await _iProductService.GetProductById(id);
            return Ok(response);
        }
        [HttpPut(ApiEndPointConstant.Product.ProductEndPoint)]
        public async Task<IActionResult> UpdateProduct(Guid id, UpdateProductRequest updateProductRequest)
        {
            var response = await _iProductService.UpdateProduct(id, updateProductRequest);
            if (!response) return Ok(MessageConstant.Product.UpdateProductFailedMessage);
            return Ok(MessageConstant.Product.UpdateStatusSuccessMessage);
        }
        [HttpDelete(ApiEndPointConstant.Product.ProductEndPoint)]
        public async Task<IActionResult> RemoveProductStatus(Guid id)
        {
            var response = await _iProductService.RemoveProductStatus(id);
            if (!response) return Ok(MessageConstant.Product.UpdateProductFailedMessage);
            return Ok(MessageConstant.Product.UpdateStatusSuccessMessage);

        }
        [HttpPost(ApiEndPointConstant.Product.AddSupProductToProductEndPoint)]
        public async Task<IActionResult> AddrankForAccount(Guid id, [FromBody] List<Guid> supProductId)
        {
            var response = await _iProductService.AddSupProductToProduct(id, supProductId);
            return Ok(response);
        }
    }
}
