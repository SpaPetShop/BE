using AutoMapper;
using Azure.Core;
using Meta.BusinessTier.Constants;
using Meta.BusinessTier.Enums.Status;
using Meta.BusinessTier.Payload;
using Meta.BusinessTier.Payload.Category;
using Meta.BusinessTier.Payload.PetService;
using Meta.BusinessTier.Payload.Product;
using Meta.BusinessTier.Services.Interfaces;
using Meta.BusinessTier.Utils;
using Meta.DataTier.Models;
using Meta.DataTier.Paginate;
using Meta.DataTier.Repository.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Meta.BusinessTier.Services.Implements
{
    public class ProductService : BaseService<ProductService>, IProductService
    {
        public ProductService(IUnitOfWork<MetaContext> unitOfWork, ILogger<ProductService> logger, IMapper mapper, IHttpContextAccessor httpContextAccessor) : base(unitOfWork, logger, mapper, httpContextAccessor)
        {
        }

        public async Task<bool> AddSupProductToProduct(Guid productId, List<Guid> supProductIds)
        {
            // Retrieve the product or throw an exception if not found
            Product product = await _unitOfWork.GetRepository<Product>().SingleOrDefaultAsync(
                predicate: x => x.Id.Equals(productId))
            ?? throw new BadHttpRequestException("Product not found");

            // Retrieve current SupProduct IDs associated with the product
            List<Guid> currentSupProductIds = (await _unitOfWork.GetRepository<ProductPetService>()
                .GetListAsync(selector: x => x.SupProductId, predicate: x => x.ProductId.Equals(productId)))
                .ToList();

            // Determine the IDs to add, remove, and keep
            (List<Guid> idsToRemove, List<Guid> idsToAdd, List<Guid> idsToKeep) splittedSupProductIds =
                CustomListUtil.splitidstoaddandremove(currentSupProductIds, supProductIds);

            // Add new SupProducts
            if (splittedSupProductIds.idsToAdd.Count > 0)
            {
                List<ProductPetService> supProductsToInsert = splittedSupProductIds.idsToAdd
                    .Select(supProductId => new ProductPetService
                    {
                        Id = Guid.NewGuid(),
                        ProductId = productId,
                        SupProductId = supProductId,
                    }).ToList();

                await _unitOfWork.GetRepository<ProductPetService>().InsertRangeAsync(supProductsToInsert);
            }

            // Remove obsolete SupProducts
            if (splittedSupProductIds.idsToRemove.Count > 0)
            {
                List<ProductPetService> ranksToDelete = (List<ProductPetService>)await _unitOfWork.GetRepository<ProductPetService>()
                    .GetListAsync(predicate: x =>
                        x.ProductId.Equals(productId) &&
                        splittedSupProductIds.idsToRemove.Contains(x.SupProductId));

                _unitOfWork.GetRepository<ProductPetService>().DeleteRangeAsync(ranksToDelete);
            }

            // Commit the changes to the database
            bool isSuccessful = await _unitOfWork.CommitAsync() > 0;
            return isSuccessful;
        }

        public async Task<Guid> CreateNewProducts(CreateNewProductRequest createNewProductRequest)
        {
            var currentUser = GetUsernameFromJwt();
            Account account = await _unitOfWork.GetRepository<Account>().SingleOrDefaultAsync(
                predicate: x => x.Username.Equals(currentUser));
            DateTime currentTime = TimeUtils.GetCurrentSEATime();

            Product existingProduct = await _unitOfWork.GetRepository<Product>().SingleOrDefaultAsync(
                predicate: x => x.Name.Equals(createNewProductRequest.Name));
            if (existingProduct != null)
            {
                throw new BadHttpRequestException(MessageConstant.Product.ProductNameExisted);
            }

            Category category = await _unitOfWork.GetRepository<Category>().SingleOrDefaultAsync(
                predicate: x => x.Id.Equals(createNewProductRequest.CategoryId));
            if (category == null)
            {
                throw new BadHttpRequestException(MessageConstant.Category.NotFoundFailedMessage);
            }
            var newProduct = new Product
            {
                Id = Guid.NewGuid(),
                Name = createNewProductRequest.Name,
                StockPrice = createNewProductRequest.StockPrice,
                SellingPrice = createNewProductRequest.SellingPrice,
                Description = createNewProductRequest.Description,
                Status = createNewProductRequest.Status.GetDescriptionFromEnum(),
                Priority = createNewProductRequest.Priority,
                CategoryId = createNewProductRequest.CategoryId,
                CreateDate = currentTime
            };
            var imagesUrl = new List<ProductImage>();
            foreach (var img in createNewProductRequest.Image)
            {
                imagesUrl.Add(new ProductImage
                {
                    Id = Guid.NewGuid(),
                    ImageUrl = img.ImageURL,
                    ProductId = newProduct.Id,

                });
            };


            await _unitOfWork.GetRepository<Product>().InsertAsync(newProduct);
            bool isSuccess = await _unitOfWork.CommitAsync() > 0;

            if (!isSuccess)
            {
                throw new BadHttpRequestException(MessageConstant.Product.CreateNewProductFailedMessage);
            }
            if (createNewProductRequest.supProductId != null && createNewProductRequest.supProductId.Count > 0)
            {
                bool componentsAdded = await AddSupProductToProduct(newProduct.Id, createNewProductRequest.supProductId);
                if (!componentsAdded)
                {
                    throw new BadHttpRequestException(MessageConstant.MachineryComponents.CreateNewMachineryComponentsFailedMessage);
                }
            }
            return newProduct.Id;
        }


        public async Task<GetProductsResponse> GetProductById(Guid id)
        {
            if (id == Guid.Empty)
                throw new BadHttpRequestException(MessageConstant.Product.EmptyProductIdMessage);

            Product product = await _unitOfWork.GetRepository<Product>().SingleOrDefaultAsync(
                predicate: x => x.Id.Equals(id),
                include: x => x.Include(p => p.Category)
                               .Include(s => s.ProductPetServices)
                               .ThenInclude(sup => sup.SupProduct)
                               .Include(x => x.ProductImages))
            ?? throw new BadHttpRequestException(MessageConstant.Product.ProductNotFoundMessage);

            GetProductsResponse response = new GetProductsResponse
            {
                Id = product.Id,
                Name = product.Name,
                StockPrice = product.StockPrice,
                SellingPrice = product.SellingPrice,
                Description = product.Description,
                Status = EnumUtil.ParseEnum<ProductStatus>(product.Status),
                Priority = product.Priority,
                Category = new CategoryResponse
                {
                    Id = (Guid)product.CategoryId,
                    Name = product.Category.Name
                },
                SupProducts = product.ProductPetServices.Select(sup => new SupProductResponse
                {
                    Id = sup.SupProduct.Id,
                    Name = sup.SupProduct.Name,
                    SellingPrice = sup.SupProduct.SellingPrice,
                    StockPrice = sup.SupProduct.StockPrice

                }).ToList(),
                Image = product.ProductImages.Select(image => new ImageResponse
                {
                    ImageURL = image.ImageUrl
                }).ToList(),
            };

            return response;
        }


        public async Task<IPaginate<GetProductsResponse>> GetProductList(ProductFilter filter, PagingModel pagingModel)
        {
            IPaginate<GetProductsResponse> productList = await _unitOfWork.GetRepository<Product>().GetPagingListAsync(
                selector: x => new GetProductsResponse
                {
                    Id = x.Id,
                    Name = x.Name,
                    StockPrice = x.StockPrice,
                    SellingPrice = x.SellingPrice,
                    Description = x.Description,
                    Status = EnumUtil.ParseEnum<ProductStatus>(x.Status),
                    Priority = x.Priority,
                    Category = new CategoryResponse
                    {
                        Id = x.Id,
                        Name = x.Category.Name
                    },
                    SupProducts = x.ProductPetServices.Select(sup => new SupProductResponse
                    {
                        Id = sup.SupProduct.Id,
                        Name = sup.SupProduct.Name,
                        SellingPrice = sup.SupProduct.SellingPrice,
                        StockPrice = sup.SupProduct.StockPrice
                    }).ToList(),
                    Image = x.ProductImages.Select(image => new ImageResponse
                    {
                        ImageURL = image.ImageUrl
                    }).ToList(),

                },
                filter: filter,
                orderBy: x => x.OrderByDescending(x => x.CreateDate),
                include: x => x.Include(p => p.Category)
                               .Include(s => s.ProductPetServices)
                               .ThenInclude(sup => sup.SupProduct)
                               .Include(x => x.ProductImages),
                page: pagingModel.page,
                size: pagingModel.size
            ) ?? throw new BadHttpRequestException(MessageConstant.Product.ProductNotFoundMessage);

            return productList;
        }


        public async Task<bool> RemoveProductStatus(Guid id)
        {
            if (id == Guid.Empty) throw new BadHttpRequestException(MessageConstant.Product.EmptyProductIdMessage);
            Product product = await _unitOfWork.GetRepository<Product>().SingleOrDefaultAsync(
                predicate: x => x.Id.Equals(id))
                ?? throw new BadHttpRequestException(MessageConstant.Product.ProductNotFoundMessage);
            product.Status = ProductStatus.UnAvailable.GetDescriptionFromEnum();
            _unitOfWork.GetRepository<Product>().UpdateAsync(product);
            bool isSuccessful = await _unitOfWork.CommitAsync() > 0;
            return isSuccessful;
        }

        public async Task<bool> UpdateProduct(Guid id, UpdateProductRequest updateProductRequest)
        {
            if (id == Guid.Empty)
                throw new BadHttpRequestException(MessageConstant.Product.EmptyProductIdMessage);

            Product product = await _unitOfWork.GetRepository<Product>().SingleOrDefaultAsync(
                predicate: x => x.Id.Equals(id))
            ?? throw new BadHttpRequestException(MessageConstant.Product.ProductNotFoundMessage);

            // Validate and retrieve the category if needed
            if (updateProductRequest.CategoryId != Guid.Empty)
            {
                Category category = await _unitOfWork.GetRepository<Category>().SingleOrDefaultAsync(
                    predicate: x => x.Id.Equals(updateProductRequest.CategoryId))
                ?? throw new BadHttpRequestException(MessageConstant.Category.NotFoundFailedMessage);

                product.Category = category;
            }

            // Update fields
            product.Name = string.IsNullOrEmpty(updateProductRequest.Name) ? product.Name : updateProductRequest.Name;
            product.StockPrice = updateProductRequest.StockPrice.HasValue ? updateProductRequest.StockPrice.Value : product.StockPrice;
            product.SellingPrice = updateProductRequest.SellingPrice.HasValue ? updateProductRequest.SellingPrice.Value : product.SellingPrice;
            product.Description = string.IsNullOrEmpty(updateProductRequest.Description) ? product.Description : updateProductRequest.Description;
            product.Priority = updateProductRequest.Priority.HasValue ? updateProductRequest.Priority.Value : product.Priority;

            // Update the status if provided
            if (updateProductRequest.Status != null)
            {
                product.Status = updateProductRequest.Status.GetDescriptionFromEnum();
            }
            else
            {
                throw new BadHttpRequestException(MessageConstant.Status.ExsitingValue);
            }

            // Mark the product as updated
            _unitOfWork.GetRepository<Product>().UpdateAsync(product);
            bool isSuccess = await _unitOfWork.CommitAsync() > 0;
            return isSuccess;
        }

    }
}
