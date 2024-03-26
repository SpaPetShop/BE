﻿using AutoMapper;
using Azure.Core;
using Meta.BusinessTier.Constants;
using Meta.BusinessTier.Enums;
using Meta.BusinessTier.Payload;
using Meta.BusinessTier.Payload.Category;
using Meta.BusinessTier.Payload.User;
using Meta.BusinessTier.Utils;
using Meta.DataTier.Models;
using Meta.DataTier.Paginate;
using Meta.DataTier.Repository.Interfaces;
using Microsoft.AspNetCore.Http;
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
        public CategoryService(IUnitOfWork<MetaContext> unitOfWork, ILogger<CategoryService> logger, IMapper mapper, IHttpContextAccessor httpContextAccessor) : base(unitOfWork, logger, mapper, httpContextAccessor)
        {
        }

        public async Task<Guid> CreateNewCategory(CreateNewCategoryRequest createNewCategoryRequest)
        {
            Category category = await _unitOfWork.GetRepository<Category>().SingleOrDefaultAsync(
                predicate: x => x.Name.Equals(createNewCategoryRequest.Name));
            if (category != null) throw new BadHttpRequestException(MessageConstant.Category.CategoryExistedMessage);
            category = _mapper.Map<Category>(createNewCategoryRequest);
            await _unitOfWork.GetRepository<Category>().InsertAsync(category);
            bool isSuccess = await _unitOfWork.CommitAsync() > 0;
            if (!isSuccess) throw new BadHttpRequestException(MessageConstant.Category.CreateCategoryFailedMessage);
            return category.Id;
        }

        public async Task<IPaginate<GetCategoriesResponse>> GetCategories(CategoryFilter filter, PagingModel pagingModel)
        {
            IPaginate<GetCategoriesResponse> respone = await _unitOfWork.GetRepository<Category>().GetPagingListAsync(
               selector: x => _mapper.Map<GetCategoriesResponse>(x),
               filter: filter,
               page: pagingModel.page,
               size: pagingModel.size,
               orderBy: x => x.OrderBy(x => x.Priority));
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
                predicate: x => x.Id.Equals(id))
                ?? throw new BadHttpRequestException(MessageConstant.Category.NotFoundFailedMessage);
            category.Status = CategoryStatus.Inactive.GetDescriptionFromEnum();
            _unitOfWork.GetRepository<Category>().UpdateAsync(category);
            bool isSuccessful = await _unitOfWork.CommitAsync() > 0;
            return isSuccessful;
        }

        public async Task<bool> UpdateCategory(Guid id, UpdateCategoryRequest updateCategoryRequest)
        {
            if (id == Guid.Empty) throw new BadHttpRequestException(MessageConstant.Category.CategoryEmptyMessage);
            Category category = await _unitOfWork.GetRepository<Category>().SingleOrDefaultAsync(
                predicate: x => x.Id.Equals(id))
            ?? throw new BadHttpRequestException(MessageConstant.Category.CategoryExistedMessage);
            category.Name = string.IsNullOrEmpty(updateCategoryRequest.Name) ? category.Name : updateCategoryRequest.Name;
            category.Description = string.IsNullOrEmpty(updateCategoryRequest.Description) ? category.Description : updateCategoryRequest.Description;
            category.Status = updateCategoryRequest.Status.GetDescriptionFromEnum();
            category.Priority = updateCategoryRequest.Priority;
            _unitOfWork.GetRepository<Category>().UpdateAsync(category);
            bool isSuccess = await _unitOfWork.CommitAsync() > 0;
            return isSuccess;

        }
    }
}
