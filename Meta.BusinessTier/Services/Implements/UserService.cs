﻿using AutoMapper;
using Meta.BusinessTier.Constants;
using Meta.BusinessTier.Enums;
using Meta.BusinessTier.Payload;
using Meta.BusinessTier.Payload.Login;
using Meta.BusinessTier.Payload.User;
using Meta.BusinessTier.Services.Interfaces;
using Meta.BusinessTier.Utils;
using Meta.DataTier.Models;
using Meta.DataTier.Paginate;
using Meta.DataTier.Repository.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Meta.BusinessTier.Services.Implements
{
    public class UserService : BaseService<UserService>, IUserService
    {
        public UserService(IUnitOfWork<MetaContext> unitOfWork, ILogger<UserService> logger, IMapper mapper, IHttpContextAccessor httpContextAccessor) : base(unitOfWork, logger, mapper, httpContextAccessor)
        {
        }

        public async Task<Guid> CreateNewUser(CreateNewUserRequest request)
        {
            User user = await _unitOfWork.GetRepository<User>().SingleOrDefaultAsync(
                predicate: x => x.Username.Equals(request.Username));
            if (user != null) throw new BadHttpRequestException(MessageConstant.User.UserExisted);
            user = new User()
            {
                Id = Guid.NewGuid(),
                Username = request.Username,
                Password = PasswordUtil.HashPassword(request.Password),
                Role = request.Role.GetDescriptionFromEnum(),
                Status = UserStatus.Activate.GetDescriptionFromEnum()
            };

            await _unitOfWork.GetRepository<User>().InsertAsync(user);
            bool isSuccessful = await _unitOfWork.CommitAsync() > 0;
            if (!isSuccessful) throw new BadHttpRequestException(MessageConstant.User.CreateFailedMessage);
            return user.Id;
        }

        public async Task<IPaginate<GetUsersResponse>> GetAllUsers(UserFilter filter, PagingModel pagingModel)
        {
            IPaginate<GetUsersResponse> respone = await _unitOfWork.GetRepository<User>().GetPagingListAsync(
               selector: x => _mapper.Map<GetUsersResponse>(x),
               filter: filter,
               page: pagingModel.page,
               size: pagingModel.size,
               orderBy: x => x.OrderBy(x => x.Username));
            return respone;
        }

        public async Task<GetUsersResponse> GetUserById(Guid id)
        {
            if (id == Guid.Empty) throw new BadHttpRequestException(MessageConstant.User.EmptyUserIdMessage);
            User user = await _unitOfWork.GetRepository<User>().SingleOrDefaultAsync(
                predicate: x => x.Id.Equals(id))
                ?? throw new BadHttpRequestException(MessageConstant.User.UserNotFoundMessage);
            return _mapper.Map<GetUsersResponse>(user);
        }

        public async Task<LoginResponse> Login(LoginRequest loginRequest)
        {
            User user = await _unitOfWork.GetRepository<User>().SingleOrDefaultAsync(
                predicate: x => x.Username.Equals(loginRequest.Username));
            if (user == null || !PasswordUtil.VerifyHashedPassword(user.Password, loginRequest.Password))
            {
                return null;
            }

            var tokenModel = JwtUtil.GenerateJwtToken(user);
            var loginResponse = new LoginResponse
            {
                Username = loginRequest.Username,
                Role = EnumUtil.ParseEnum<RoleEnum>(user.Role),
                Status = EnumUtil.ParseEnum<UserStatus>(user.Status),
                Id = user.Id,
                TokenModel = tokenModel,


            };
            return loginResponse;
        }

        public async Task<bool> RemoveUserStatus(Guid id)
        {
            if (id == Guid.Empty) throw new BadHttpRequestException(MessageConstant.User.EmptyUserIdMessage);
            User user = await _unitOfWork.GetRepository<User>().SingleOrDefaultAsync(
                predicate: x => x.Id.Equals(id))
                ?? throw new BadHttpRequestException(MessageConstant.User.UserNotFoundMessage);
            user.Status = UserStatus.Deactivate.GetDescriptionFromEnum();
            _unitOfWork.GetRepository<User>().UpdateAsync(user);
            bool isSuccessful = await _unitOfWork.CommitAsync() > 0;
            return isSuccessful;
        }

        public async Task<bool> UpdateUserInfor(Guid id, UpdateUserInforRequest updateRequest)
        {
            if (id == Guid.Empty) throw new BadHttpRequestException(MessageConstant.User.EmptyUserIdMessage);
            User user = await _unitOfWork.GetRepository<User>().SingleOrDefaultAsync(
                predicate: x => x.Id.Equals(id))
                ?? throw new BadHttpRequestException(MessageConstant.User.UserNotFoundMessage);
            user.Password = string.IsNullOrEmpty(updateRequest.Password) ? user.Password : PasswordUtil.HashPassword(updateRequest.Password);
            user.Status = updateRequest.Status.GetDescriptionFromEnum();
            _unitOfWork.GetRepository<User>().UpdateAsync(user);
            bool isSuccessful  = await _unitOfWork.CommitAsync() > 0;
            return isSuccessful;
        }
    }
}
