//using DentalLabManagement.API.Controllers;
//using Meta.BusinessTier.Constants;
//using Meta.BusinessTier.Payload.Category;
//using Meta.BusinessTier.Payload;
//using Meta.BusinessTier.Services.Implements;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;

//namespace Meta.API.Controllers
//{

//    [ApiController]
//    public class TypePetController : BaseController<TypePetController>
//    {
//        readonly ITypeService _typeService;

//        public TypePetController(ILogger<TypePetController> logger, ITypeService typeService) : base(logger)
//        {
//            _typeService = typeService;
//        }
//        [HttpPost(ApiEndPointConstant.Category.CategoriesEndPoint)]
//        public async Task<IActionResult> CreateNewCategory(CreateNewCategoryRequest category)
//        {
//            var response = await _categoryService.CreateNewCategory(category);
//            return Ok(response);
//        }
//        [HttpGet(ApiEndPointConstant.Category.CategoriesEndPoint)]
//        public async Task<IActionResult> GetCategories([FromQuery] CategoryFilter filter, [FromQuery] PagingModel pagingModel)
//        {
//            var response = await _categoryService.GetCategories(filter, pagingModel);
//            return Ok(response);
//        }
//        [HttpGet(ApiEndPointConstant.Category.CategoriesNopagingNateEndPoint)]
//        public async Task<IActionResult> GetCategoriesNoPagingNate([FromQuery] CategoryFilter filter)
//        {
//            var response = await _categoryService.GetCategoriesNoPagingNate(filter);
//            return Ok(response);
//        }
//        [HttpGet(ApiEndPointConstant.Category.CategoryEndPoint)]
//        public async Task<IActionResult> GetCategory(Guid id)
//        {
//            var response = await _categoryService.GetCategory(id);
//            return Ok(response);
//        }
//        [HttpPut(ApiEndPointConstant.Category.CategoryEndPoint)]
//        public async Task<IActionResult> UpdateCategory(Guid id, UpdateCategoryRequest updateCategoryRequest)
//        {
//            var isSuccessful = await _categoryService.UpdateCategory(id, updateCategoryRequest);
//            if (!isSuccessful) return Ok(MessageConstant.Category.UpdateCategoryFailedMessage);
//            return Ok(MessageConstant.Category.UpdateCategorySuccessMessage);
//        }
//        [HttpDelete(ApiEndPointConstant.Category.CategoryEndPoint)]
//        public async Task<IActionResult> RemoveCategoryStatus(Guid id)
//        {
//            var isSuccessful = await _categoryService.RemoveCategoryStatus(id);
//            if (!isSuccessful) return Ok(MessageConstant.Category.UpdateCategoryFailedMessage);
//            return Ok(MessageConstant.Category.UpdateCategorySuccessMessage);
//        }
//    }
//}
