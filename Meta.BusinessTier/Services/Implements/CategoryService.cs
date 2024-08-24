using AutoMapper;
using Azure.Core;
using Meta.BusinessTier.Constants;
using Meta.BusinessTier.Enums.Status;
using Meta.BusinessTier.Enums.Type;
using Meta.BusinessTier.Payload;
using Meta.BusinessTier.Payload.Category;
using Meta.BusinessTier.Payload.User;
using Meta.BusinessTier.Utils;
using Meta.DataTier.Models;
using Meta.DataTier.Paginate;
using Meta.DataTier.Repository.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meta.BusinessTier.Services.Implements
{
    public class CategoryService : BaseService<CategoryService>, ICategoryService
    {
        public CategoryService(IUnitOfWork<SpaPetContext> unitOfWork, ILogger<CategoryService> logger, IMapper mapper, IHttpContextAccessor httpContextAccessor) : base(unitOfWork, logger, mapper, httpContextAccessor)
        {
        }

        public async Task<Guid> CreateNewCategory(CreateNewCategoryRequest request)
        {

            Category newCategory = _mapper.Map<Category>(request);
            newCategory.Id = Guid.NewGuid();
            newCategory.Status = CategoryStatus.ACTIVE.GetDescriptionFromEnum();


            await _unitOfWork.GetRepository<Category>().InsertAsync(newCategory);

            bool isSuccessful = await _unitOfWork.CommitAsync() > 0;
            if (!isSuccessful) throw new BadHttpRequestException(MessageConstant.Category.CreateCategoryFailedMessage);

            return newCategory.Id;
        }

        public async Task<IPaginate<GetCategoriesResponse>> GetCategories(CategoryFilter filter, PagingModel pagingModel)
        {
            IPaginate<GetCategoriesResponse> respone = await _unitOfWork.GetRepository<Category>().GetPagingListAsync(
               selector: x => _mapper.Map<GetCategoriesResponse>(x),
               filter: filter,
               page: pagingModel.page,
               size: pagingModel.size);
            return respone;
        }
        public async Task<ICollection<GetCategoriesResponse>> GetCategoriesNoPagingNate(CategoryFilter filter)
        {
            ICollection<GetCategoriesResponse> respone = await _unitOfWork.GetRepository<Category>().GetListAsync(
               selector: x => _mapper.Map<GetCategoriesResponse>(x),
               filter: filter);
            return respone;

        }

        public async Task<GetCategoriesResponse> GetCategory(Guid id)
        {
            if (id == Guid.Empty) throw new BadHttpRequestException(MessageConstant.Category.CategoryEmptyMessage);
            Category category = await _unitOfWork.GetRepository<Category>().SingleOrDefaultAsync(
                predicate: x => x.Id.Equals(id))
                ?? throw new BadHttpRequestException(MessageConstant.Category.NotFoundFailedMessage);
            return _mapper.Map<GetCategoriesResponse>(category);
        }

        public async Task<bool> RemoveCategoryStatus(Guid id)
        {
            if (id == Guid.Empty) throw new BadHttpRequestException(MessageConstant.Category.CategoryEmptyMessage);
            Category category = await _unitOfWork.GetRepository<Category>().SingleOrDefaultAsync(
                predicate: x => x.Id.Equals(id),
                include: x => x.Include(x => x.Products))
                ?? throw new BadHttpRequestException(MessageConstant.Category.NotFoundFailedMessage);
            foreach (var item in category.Products)
            {
                item.Status = ProductStatus.UNAVAILABLE.GetDescriptionFromEnum();
            }
            //foreach (var item in category.Products)
            //{
            //    item.Status = ProductStatus.InActive.GetDescriptionFromEnum();
            //}
            category.Status = CategoryStatus.INACTIVE.GetDescriptionFromEnum();
            _unitOfWork.GetRepository<Category>().UpdateAsync(category);
            bool isSuccessful = await _unitOfWork.CommitAsync() > 0;
            return isSuccessful;
        }

        public async Task<bool> UpdateCategory(Guid id, UpdateCategoryRequest updateCategoryRequest)
        {
            if (id == Guid.Empty) throw new BadHttpRequestException(MessageConstant.Category.CategoryEmptyMessage);
            Category category = await _unitOfWork.GetRepository<Category>().SingleOrDefaultAsync(
                predicate: x => x.Id.Equals(id),
                include: x => x.Include(x => x.Products))
                ?? throw new BadHttpRequestException(MessageConstant.Category.NotFoundFailedMessage);


            category.Name = string.IsNullOrEmpty(updateCategoryRequest.Name) ? category.Name : updateCategoryRequest.Name;
            category.Description = string.IsNullOrEmpty(updateCategoryRequest.Description) ? category.Description : updateCategoryRequest.Description;
            switch (updateCategoryRequest.Status)
            {
                case CategoryStatus.ACTIVE:
                    category.Status = CategoryStatus.ACTIVE.GetDescriptionFromEnum();
                    foreach (var item in category.Products)
                    {
                        item.Status = ProductStatus.AVAILABLE.GetDescriptionFromEnum();
                    }
                    //foreach (var item in category.MachineComponents)
                    //{
                    //    item.Status = MachineryStatus.Available.GetDescriptionFromEnum();
                    //}
                    break;
                case CategoryStatus.INACTIVE:
                    category.Status = CategoryStatus.INACTIVE.GetDescriptionFromEnum();
                    foreach (var item in category.Products)
                    {
                        item.Status = ProductStatus.UNAVAILABLE.GetDescriptionFromEnum();
                    }
                    //foreach (var item in updateCategory.MachineComponents)
                    //{
                    //    item.Status = ComponentStatus.InActive.GetDescriptionFromEnum();
                    //}

                    break;
                default:
                    return false;
            }
            _unitOfWork.GetRepository<Category>().UpdateAsync(category);
            bool isSuccess = await _unitOfWork.CommitAsync() > 0;
            return isSuccess;

        }

    }
}
