using Meta.BusinessTier.Payload.Category;
using Meta.BusinessTier.Payload;
using Meta.DataTier.Paginate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Meta.BusinessTier.Payload.Product;

namespace Meta.BusinessTier.Services.Interfaces
{
    public interface IProductService
    {
        Task<Guid> CreateNewProducts(CreateNewProductRequest createNewProductRequest);
        Task<bool> UpdateProduct(Guid id, UpdateProductRequest updateProductRequest);
        Task<IPaginate<GetProductsResponse>> GetProductList(ProductFilter filter, PagingModel pagingModel);
        Task<GetProductsResponse> GetProductById(Guid id);
        Task<bool> RemoveProductStatus(Guid id);
        Task<bool> AddSupProductToProduct(Guid id, List<Guid> request);
    }
}
