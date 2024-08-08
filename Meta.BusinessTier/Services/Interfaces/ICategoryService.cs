using Meta.BusinessTier.Payload;
using Meta.BusinessTier.Payload.Category;
using Meta.BusinessTier.Payload.User;
using Meta.DataTier.Paginate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meta.BusinessTier.Services.Implements
{
    public interface ICategoryService
    {
        Task<Guid> CreateNewCategory(CreateNewCategoryRequest createNewCategoryRequest);
        Task<bool> UpdateCategory(Guid id, UpdateCategoryRequest updateCategoryRequest);
        Task<IPaginate<GetCategoriesResponse>> GetCategories(CategoryFilter filter, PagingModel pagingModel);
        Task<ICollection<GetCategoriesResponse>> GetCategoriesNoPagingNate(CategoryFilter filter);
        Task<GetCategoriesResponse> GetCategory(Guid id);
        Task<bool> RemoveCategoryStatus(Guid id);
    }
}
