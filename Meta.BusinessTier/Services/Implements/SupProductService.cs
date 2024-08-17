using AutoMapper;
using Meta.BusinessTier.Constants;
using Meta.BusinessTier.Enums.Status;
using Meta.BusinessTier.Payload;
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;

namespace Meta.BusinessTier.Services.Implements
{
    public class SupProductService : BaseService<SupProductService>, ISupProductService
    {
        public SupProductService(IUnitOfWork<MetaContext> unitOfWork, ILogger<SupProductService> logger, IMapper mapper, IHttpContextAccessor httpContextAccessor) : base(unitOfWork, logger, mapper, httpContextAccessor)
        {
        }

        public async Task<Guid> CreateNewPSupProduct(CreateNewSupProductRequest request)
        {
            var currentUser = GetUsernameFromJwt();
            Account account = await _unitOfWork.GetRepository<Account>().SingleOrDefaultAsync(
                predicate: x => x.Username.Equals(currentUser));
            DateTime currentTime = TimeUtils.GetCurrentSEATime();

            var existingSupProduct = await _unitOfWork.GetRepository<SupProduct>().SingleOrDefaultAsync(
                predicate: x => x.Name.Equals(request.Name));
            if (existingSupProduct != null)
            {
                throw new BadHttpRequestException(MessageConstant.Product.ProductNameExisted);
            }

            Category category = await _unitOfWork.GetRepository<Category>().SingleOrDefaultAsync(
                predicate: x => x.Id.Equals(request.CategoryId));
            if (category == null)
            {
                throw new BadHttpRequestException(MessageConstant.Category.NotFoundFailedMessage);
            }

            var subProductService = new SupProduct
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                StockPrice = request.StockPrice,
                SellingPrice = request.SellingPrice,
                Desctiprion = request.Description,
                Status = request.Status.GetDescriptionFromEnum(),
                CreateDate = currentTime,
                CategoryId = request.CategoryId
            };
            var imagesUrl = new List<SupProductImage>();
            foreach (var img in request.Image)
            {
                imagesUrl.Add(new SupProductImage
                {
                    Id = Guid.NewGuid(),
                    ImageUrl = img.ImageURL,
                    SupProductId = subProductService.Id,

                });
            };

            await _unitOfWork.GetRepository<SupProduct>().InsertAsync(subProductService);
            await _unitOfWork.GetRepository<SupProductImage>().InsertRangeAsync(imagesUrl);
            bool isSuccess = await _unitOfWork.CommitAsync() > 0;

            if (!isSuccess)
            {
                throw new BadHttpRequestException(MessageConstant.Product.CreateNewProductFailedMessage);
            }
            return subProductService.Id;
        }

        public async Task<GetSupProductsResponse> GetSupProductById(Guid id)
        {
            if (id == Guid.Empty)
                throw new BadHttpRequestException(MessageConstant.Product.EmptyProductIdMessage);

            SupProduct supProduct = await _unitOfWork.GetRepository<SupProduct>().SingleOrDefaultAsync(
                predicate: x => x.Id.Equals(id),
                include: x => x.Include(sp => sp.Category)
                                .Include(x => x.SupProductImages))
            ?? throw new BadHttpRequestException(MessageConstant.Product.ProductNotFoundMessage);

            return new GetSupProductsResponse
            {
                Id = supProduct.Id,
                Name = supProduct.Name,
                StockPrice = supProduct.StockPrice,
                SellingPrice = supProduct.SellingPrice,
                Description = supProduct.Desctiprion,
                Status = EnumUtil.ParseEnum<ProductStatus>(supProduct.Status),
                Category = new CategoryResponse
                {
                    Id = (Guid)supProduct.CategoryId,
                    Name = supProduct.Category.Name
                },
                Image = supProduct.SupProductImages.Select(image => new ImageResponse
                {
                    ImageURL = image.ImageUrl
                }).ToList(),
            };
        }

        public async Task<IPaginate<GetSupProductsResponse>> GetSupProductList(SupProductFilter filter, PagingModel pagingModel)
        {
            return await _unitOfWork.GetRepository<SupProduct>().GetPagingListAsync(
                selector: x => new GetSupProductsResponse
                {
                    Id = x.Id,
                    Name = x.Name,
                    StockPrice = x.StockPrice,
                    SellingPrice = x.SellingPrice,
                    Description = x.Desctiprion,
                    Status = EnumUtil.ParseEnum<ProductStatus>(x.Status),
                    Category = new CategoryResponse
                    {
                        Id = (Guid)x.CategoryId,
                        Name = x.Category.Name
                    },
                    Image = x.SupProductImages.Select(image => new ImageResponse
                    {
                        ImageURL = image.ImageUrl
                    }).ToList(),

                },
                filter: filter,
                orderBy: x => x.OrderByDescending(x => x.CreateDate),
                include: x => x.Include(sp => sp.Category)
                                .Include(x => x.SupProductImages),
                page: pagingModel.page,
                size: pagingModel.size
            ) ?? throw new BadHttpRequestException(MessageConstant.Product.ProductNotFoundMessage);
        }


        public async Task<bool> RemoveSupProductStatus(Guid id)
        {
            if (id == Guid.Empty) throw new BadHttpRequestException(MessageConstant.Product.EmptyProductIdMessage);
            SupProduct supProduct = await _unitOfWork.GetRepository<SupProduct>().SingleOrDefaultAsync(
                predicate: x => x.Id.Equals(id))
                ?? throw new BadHttpRequestException(MessageConstant.Product.ProductNotFoundMessage);
            supProduct.Status = ProductStatus.UnAvailable.GetDescriptionFromEnum();
            _unitOfWork.GetRepository<SupProduct>().UpdateAsync(supProduct);
            return await _unitOfWork.CommitAsync() > 0;
        }

        public async Task<bool> UpdateSupProduct(Guid id, UpdateSupProductRequest request)
        {
            if (id == Guid.Empty)
                throw new BadHttpRequestException(MessageConstant.Product.EmptyProductIdMessage);

            SupProduct supProduct = await _unitOfWork.GetRepository<SupProduct>().SingleOrDefaultAsync(
                predicate: x => x.Id.Equals(id))
            ?? throw new BadHttpRequestException(MessageConstant.Product.ProductNotFoundMessage);

            if (request.CategoryId != Guid.Empty)
            {
                Category category = await _unitOfWork.GetRepository<Category>().SingleOrDefaultAsync(
                    predicate: x => x.Id.Equals(request.CategoryId))
                ?? throw new BadHttpRequestException(MessageConstant.Category.NotFoundFailedMessage);

                supProduct.Category = category;
            }

            supProduct.Name = string.IsNullOrEmpty(request.Name) ? supProduct.Name : request.Name;
            supProduct.StockPrice = request.StockPrice.HasValue ? request.StockPrice.Value : supProduct.StockPrice;
            supProduct.SellingPrice = request.SellingPrice.HasValue ? request.SellingPrice.Value : supProduct.SellingPrice;
            supProduct.Desctiprion = string.IsNullOrEmpty(request.Description) ? supProduct.Desctiprion : request.Description;

            if (request.Status != null)
            {
                supProduct.Status = request.Status.GetDescriptionFromEnum();
            }
            else
            {
                throw new BadHttpRequestException(MessageConstant.Status.ExsitingValue);
            }

            _unitOfWork.GetRepository<SupProduct>().UpdateAsync(supProduct);
            return await _unitOfWork.CommitAsync() > 0;
        }
    }
}
