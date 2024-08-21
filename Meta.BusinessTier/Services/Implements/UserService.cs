﻿using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Meta.BusinessTier.Constants;
using Meta.BusinessTier.Payload.Login;
using Meta.BusinessTier.Payload.User;
using Meta.BusinessTier.Payload;
using Meta.BusinessTier.Services.Interfaces;
using Meta.BusinessTier.Services;
using Meta.BusinessTier.Utils;
using Meta.DataTier.Paginate;
using Meta.DataTier.Repository.Interfaces;
using Meta.DataTier.Models;
using Meta.BusinessTier.Enums.Status;
using Meta.BusinessTier.Extensions;

namespace Meta.BusinessTier.Services.Implements
{
    public class UserService : BaseService<UserService>, IUserService
    {
        public UserService(IUnitOfWork<MetaContext> unitOfWork, ILogger<UserService> logger, IMapper mapper, IHttpContextAccessor httpContextAccessor) : base(unitOfWork, logger, mapper, httpContextAccessor)
        {
        }

        public async Task<bool> AddRankToAccount(Guid id, List<Guid> request)
        {
            _logger.LogInformation($"Add Rank to Customer: {id}");

            // Retrieve the account or throw an exception if not found
            Account account = await _unitOfWork.GetRepository<Account>().SingleOrDefaultAsync(
                predicate: x => x.Id.Equals(id))
            ?? throw new BadHttpRequestException(MessageConstant.Account.NotFoundFailedMessage);

            // Retrieve current rank IDs associated with the account
            List<Guid> currentRankIds = (List<Guid>)await _unitOfWork.GetRepository<AccountRank>().GetListAsync(
                selector: x => x.RankId,
                predicate: x => x.AccountId.Equals(id));

            // Determine the IDs to add, remove, and keep
            (List<Guid> idsToRemove, List<Guid> idsToAdd, List<Guid> idsToKeep) splittedRankIds =
                CustomListUtil.splitidstoaddandremove(currentRankIds, request);

            // Add new ranks
            if (splittedRankIds.idsToAdd.Count > 0)
            {
                List<AccountRank> ranksToInsert = new List<AccountRank>();
                splittedRankIds.idsToAdd.ForEach(rankId => ranksToInsert.Add(new AccountRank
                {
                    Id = Guid.NewGuid(),
                    AccountId = id,
                    RankId = rankId,
                }));
                await _unitOfWork.GetRepository<AccountRank>().InsertRangeAsync(ranksToInsert);
            }

            // Remove obsolete ranks
            if (splittedRankIds.idsToRemove.Count > 0)
            {
                List<AccountRank> ranksToDelete = (List<AccountRank>)await _unitOfWork.GetRepository<AccountRank>()
                    .GetListAsync(predicate: x =>
                        x.AccountId.Equals(id) &&
                        splittedRankIds.idsToRemove.Contains(x.RankId));

                _unitOfWork.GetRepository<AccountRank>().DeleteRangeAsync(ranksToDelete);
            }

            // Commit the changes to the database
            bool isSuccessful = await _unitOfWork.CommitAsync() > 0;
            return isSuccessful;
        }

        public async Task<Guid> AddrankForAccount(Guid accountId, Guid rankId)
        {
            var currentUser = GetUsernameFromJwt();

            Account account = await _unitOfWork.GetRepository<Account>().SingleOrDefaultAsync(
                predicate: x => x.Id.Equals(accountId))
            ?? throw new BadHttpRequestException(MessageConstant.Account.NotFoundFailedMessage);
            Rank rank = await _unitOfWork.GetRepository<Rank>().SingleOrDefaultAsync(
                predicate: x => x.Id.Equals(rankId))
            ?? throw new BadHttpRequestException(MessageConstant.Account.NotFoundFailedMessage);

            AccountRank accountRank = new AccountRank()
            {
                Id = Guid.NewGuid(),
                AccountId = accountId,
                RankId = rankId,
            };


            await _unitOfWork.GetRepository<AccountRank>().InsertAsync(accountRank);
            bool isSuccessful = await _unitOfWork.CommitAsync() > 0;
            if (!isSuccessful) throw new BadHttpRequestException(MessageConstant.Rank.CreateNewRankFailedMessage);

            return (Guid)accountRank.Id;
        }

        public async Task<bool> ChangePassword(Guid userId, ChangePasswordRequest changePasswordRequest)
        {
            if (userId == Guid.Empty) throw new BadHttpRequestException(MessageConstant.User.EmptyUserIdMessage);

            Account user = await _unitOfWork.GetRepository<Account>().SingleOrDefaultAsync(
                predicate: x => x.Id.Equals(userId))
                ?? throw new BadHttpRequestException(MessageConstant.User.UserNotFoundMessage);

            // Kiểm tra mật khẩu hiện tại
            if (string.IsNullOrEmpty(changePasswordRequest.CurrentPassword) || !PasswordUtil.VerifyHashedPassword(user.Password, changePasswordRequest.CurrentPassword))
            {
                throw new BadHttpRequestException(MessageConstant.User.CheckPasswordFailed);
            }

            // Cập nhật mật khẩu mới
            user.Password = PasswordUtil.HashPassword(changePasswordRequest.NewPassword);

            // Cập nhật tài khoản
            _unitOfWork.GetRepository<Account>().UpdateAsync(user);
            bool isSuccessful = await _unitOfWork.CommitAsync() > 0;
            return isSuccessful;
        }
        public async Task<Guid> CreateNewUser(CreateNewUserRequest request)
        {
            try
            {
                Account account = await _unitOfWork.GetRepository<Account>().SingleOrDefaultAsync(
                    predicate: x => x.Username.Equals(request.Username));
                if (account != null) throw new BadHttpRequestException(MessageConstant.User.UserExisted);

                account = new Account()
                {
                    Id = Guid.NewGuid(),
                    Username = request.Username,
                    Password = PasswordUtil.HashPassword(request.Password),
                    Role = RoleEnum.USER.GetDescriptionFromEnum(),
                    FullName = request.FullName,
                    Gender = request.Gender,
                    PhoneNumber = request.PhoneNumber,
                    Status = UserStatus.ACTIVE.GetDescriptionFromEnum(),
                    Email = request.Email,
                    Image = request.Image,
                };

                await _unitOfWork.GetRepository<Account>().InsertAsync(account);

                bool isSuccessful = await _unitOfWork.CommitAsync() > 0;
                if (!isSuccessful) throw new BadHttpRequestException(MessageConstant.User.CreateFailedMessage);

                return account.Id;
            }
            catch (Exception ex)
            {
                // Log the exception details here, including inner exception if available
                // Example: _logger.LogError(ex, "An error occurred while creating a new user");
                throw new BadHttpRequestException("An error occurred while creating a new user", ex);
            }
        }

        public async Task<Guid> CreateNewStaff(CreateNewStaffRequest request)
        {
            Account account = await _unitOfWork.GetRepository<Account>().SingleOrDefaultAsync(
                predicate: x => x.Username.Equals(request.Username));
            if (account != null) throw new BadHttpRequestException(MessageConstant.User.UserExisted);
            account = new Account()
            {
                Id = Guid.NewGuid(),
                Username = request.Username,
                Password = PasswordUtil.HashPassword(request.Password),
                Role = request.Role,
                FullName = request.FullName,
                Gender = request.Gender,
                PhoneNumber = request.PhoneNumber,
                Status = UserStatus.ACTIVE.GetDescriptionFromEnum(),
                Email = request.Email,
                Image = request.Image,

            };


            await _unitOfWork.GetRepository<Account>().InsertAsync(account);
            bool isSuccessful = await _unitOfWork.CommitAsync() > 0;
            if (!isSuccessful) throw new BadHttpRequestException(MessageConstant.User.CreateFailedMessage);
            return account.Id;
        }
        //public async Task<ICollection<StaffTaskStatusResponse>> GetStaffTaskStatusesByRole()
        //{
        //    // Lấy danh sách tất cả nhân viên theo vai trò
        //    var staffList = await _unitOfWork.GetRepository<Account>()
        //        .GetListAsync(predicate: a => a.Role.Equals(RoleEnum.Technical.GetDescriptionFromEnum()));

        //    var staffTaskStatuses = new List<StaffTaskStatusResponse>();

        //    var currentDate = DateTime.UtcNow.Date;

        //    // Duyệt qua từng nhân viên
        //    foreach (var staff in staffList)
        //    {
        //        // Lấy tất cả công việc của nhân viên đó
        //        var tasks = await _unitOfWork.GetRepository<TaskManager>()
        //            .GetListAsync(predicate: t => t.AccountId == staff.Id);

        //        // Đếm số lượng công việc theo trạng thái
        //        var taskCountByStatus = tasks.CountTaskEachStatus();

        //        // Lọc công việc theo ngày hiện tại
        //        var todayTasks = tasks.Where(t => t.CreateDate.HasValue && t.CreateDate.Value.Date == currentDate.Date).ToList();

        //        // Đếm số lượng công việc trong ngày theo trạng thái
        //        var todayTaskCountByStatus = todayTasks.CountTaskEachStatus();

        //        // Thêm thông tin vào danh sách kết quả
        //        staffTaskStatuses.Add(new StaffTaskStatusResponse
        //        {
        //            StaffId = staff.Id,
        //            StaffName = staff.FullName,
        //            TaskStatusCount = taskCountByStatus,
        //            TodayTaskStatusCount = todayTaskCountByStatus
        //        });
        //    }

        //    return staffTaskStatuses;
        //}
        public async Task<IPaginate<GetUsersResponse>> GetAllUsers(UserFilter filter, PagingModel pagingModel)
        {
            // Lấy danh sách các account theo bộ lọc và phân trang
            IPaginate<GetUsersResponse> response = await _unitOfWork.GetRepository<Account>().GetPagingListAsync(
                 selector: x => _mapper.Map<GetUsersResponse>(x),
                 filter: filter,
                 page: pagingModel.page,
                 size: pagingModel.size,
                 orderBy: x => x.OrderBy(x => x.Username)
            );

            // Tạo danh sách response
            var responseList = new List<GetUsersResponse>();

            foreach (var account in response.Items)
            {
                // Lấy thông tin rank cho từng account
                AccountRank accountRank = await _unitOfWork.GetRepository<AccountRank>().SingleOrDefaultAsync(
                    predicate: x => x.AccountId.Equals(account.Id)
                );

                RankResponse rankResponse = null;

                if (accountRank != null)
                {
                    // Lấy thông tin rank từ bảng Rank
                    var rank = await _unitOfWork.GetRepository<Rank>().SingleOrDefaultAsync(
                        predicate: x => x.Id.Equals(accountRank.RankId)
                    );

                    if (rank != null)
                    {
                        rankResponse = new RankResponse
                        {
                            Name = rank.Name,
                            Range = rank.Range
                        };
                    }
                }

                // Map account sang GetUsersResponse và thêm thông tin rank
                var userResponse = _mapper.Map<GetUsersResponse>(account);
                userResponse.Rank = rankResponse;

                responseList.Add(userResponse);
            }



            return response;
        }


        public async Task<GetUsersResponse> GetUserById(Guid id)
        {
            if (id == Guid.Empty)
                throw new BadHttpRequestException(MessageConstant.User.EmptyUserIdMessage);

            // Retrieve the user or throw an exception if not found
            Account user = await _unitOfWork.GetRepository<Account>().SingleOrDefaultAsync(
                predicate: x => x.Id.Equals(id))
                ?? throw new BadHttpRequestException(MessageConstant.User.UserNotFoundMessage);

            // Retrieve the rank ID associated with the user
            AccountRank accountRank = await _unitOfWork.GetRepository<AccountRank>().SingleOrDefaultAsync(
                predicate: x => x.AccountId.Equals(id));

            RankResponse rankResponse = null;

            if (accountRank != null)
            {
                // Retrieve the rank information
                Rank rank = await _unitOfWork.GetRepository<Rank>().SingleOrDefaultAsync(
                    predicate: x => x.Id.Equals(accountRank.RankId));

                if (rank != null)
                {
                    rankResponse = new RankResponse
                    {
                        Name = rank.Name,
                        Range = rank.Range
                    };
                }
            }

            // Map the user to GetUsersResponse and include the rank information
            var response = _mapper.Map<GetUsersResponse>(user);
            response.Rank = rankResponse;

            return response;
        }

        public async Task<LoginResponse> Login(LoginRequest loginRequest)
        {
            Account user = await _unitOfWork.GetRepository<Account>().SingleOrDefaultAsync(
                predicate: x => x.Username.Equals(loginRequest.Username));
            if (user == null || !PasswordUtil.VerifyHashedPassword(user.Password, loginRequest.Password))
            {
                return null;
            }

            var tokenModel = JwtUtil.GenerateJwtToken(user);
            var loginResponse = new LoginResponse
            {
                Username = loginRequest.Username,
                FullName = user.FullName,
                Role = EnumUtil.ParseEnum<RoleEnum>(user.Role),
                Image = user.Image,
                Status = EnumUtil.ParseEnum<UserStatus>(user.Status),
                Id = user.Id,
                TokenModel = tokenModel,


            };
            return loginResponse;
        }

        public async Task<bool> RemoveUserStatus(Guid id)
        {
            if (id == Guid.Empty) throw new BadHttpRequestException(MessageConstant.User.EmptyUserIdMessage);
            Account user = await _unitOfWork.GetRepository<Account>().SingleOrDefaultAsync(
                predicate: x => x.Id.Equals(id))
                ?? throw new BadHttpRequestException(MessageConstant.User.UserNotFoundMessage);
            user.Status = UserStatus.DEACTIVE.GetDescriptionFromEnum();
            _unitOfWork.GetRepository<Account>().UpdateAsync(user);
            bool isSuccessful = await _unitOfWork.CommitAsync() > 0;
            return isSuccessful;
        }

        public async Task<bool> UpdateUserInfor(Guid id, UpdateUserInforRequest updateRequest)
        {
            if (id == Guid.Empty) throw new BadHttpRequestException(MessageConstant.User.EmptyUserIdMessage);
            Account user = await _unitOfWork.GetRepository<Account>().SingleOrDefaultAsync(
                predicate: x => x.Id.Equals(id))
                ?? throw new BadHttpRequestException(MessageConstant.User.UserNotFoundMessage);

            user.FullName = string.IsNullOrEmpty(updateRequest.FullName) ? user.FullName : updateRequest.FullName;
            user.PhoneNumber = string.IsNullOrEmpty(updateRequest.PhoneNumber) ? user.PhoneNumber : updateRequest.PhoneNumber;
            user.Email = string.IsNullOrEmpty(updateRequest.Email) ? user.Email : updateRequest.Email;
            user.Image = string.IsNullOrEmpty(updateRequest.Image) ? user.Image : updateRequest.Image;
            if (!updateRequest.Status.HasValue && !updateRequest.Role.HasValue && !updateRequest.Gender.HasValue)
            {
                throw new BadHttpRequestException(MessageConstant.Status.ExsitingValue);
            }
            else
            {
                user.Role = updateRequest.Role.GetDescriptionFromEnum();
                user.Status = updateRequest.Status.GetDescriptionFromEnum();
                user.Gender = updateRequest.Gender.GetDescriptionFromEnum();
            }
            _unitOfWork.GetRepository<Account>().UpdateAsync(user);
            bool isSuccessful = await _unitOfWork.CommitAsync() > 0;
            return isSuccessful;
        }
        public async Task<ICollection<StaffTaskStatusResponse>> GetStaffTaskStatusesByRole(DateTime targetDate)
        {
            // Lấy danh sách tất cả nhân viên theo vai trò
            var staffList = await _unitOfWork.GetRepository<Account>()
                .GetListAsync(predicate: a => a.Role.Equals(RoleEnum.STAFF.GetDescriptionFromEnum()));

            var staffTaskStatuses = new List<StaffTaskStatusResponse>();

            foreach (var staff in staffList)
            {
                var tasks = await _unitOfWork.GetRepository<TaskManager>()
                    .GetListAsync(predicate: t => t.AccountId == staff.Id);

                var taskCountByStatus = tasks.CountTaskEachStatus();

                var specifiedDateTasks = tasks.Where(t => t.ExcutionDate.HasValue && t.ExcutionDate.Value.Date == targetDate.Date).ToList();

                var specifiedDateTaskCountByStatus = specifiedDateTasks.CountTaskEachStatus();

                staffTaskStatuses.Add(new StaffTaskStatusResponse
                {
                    StaffId = staff.Id,
                    StaffName = staff.FullName,
                    TaskStatusCount = taskCountByStatus,
                    TodayTaskStatusCount = specifiedDateTaskCountByStatus
                });
            }

            return staffTaskStatuses;
        }
    }
}
