using DentalLabManagement.API.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Meta.BusinessTier.Constants;
using Meta.BusinessTier.Enums.Status;
using Meta.BusinessTier.Error;
using Meta.BusinessTier.Payload.Login;
using Meta.BusinessTier.Payload.User;
using Meta.BusinessTier.Payload;
using Meta.BusinessTier.Services.Interfaces;

namespace Meta.API.Controllers
{
    [ApiController]
    public class AccountController : BaseController<AccountController>
    {
        private readonly IUserService _userService;

        public AccountController(ILogger<AccountController> logger, IUserService userService) : base(logger)
        {
            _userService = userService;
        }
        [HttpPost(ApiEndPointConstant.Authentication.Login)]
        public async Task<IActionResult> Login(LoginRequest loginRequest)
        {
            var response = await _userService.Login(loginRequest);
            if (response == null)
            {
                return Unauthorized(new ErrorResponse
                {
                    StatusCode = StatusCodes.Status401Unauthorized,
                    Error = MessageConstant.LoginMessage.InvalidUsernameOrPassword,
                    TimeStamp = DateTime.Now
                });
            }
            if (response.Status == UserStatus.DEACTIVE)
            {
                return Unauthorized(new ErrorResponse
                {
                    StatusCode = StatusCodes.Status401Unauthorized,
                    Error = MessageConstant.LoginMessage.DeactivatedAccount,
                    TimeStamp = DateTime.Now
                });
            }
            return Ok(response);
        }

        [HttpPost(ApiEndPointConstant.Account.CustomerEndPoint)]
        public async Task<IActionResult> CreateNewUser(CreateNewUserRequest createNewUserRequest)
        {
            var response = await _userService.CreateNewUser(createNewUserRequest);
            return Ok(response);
        }
        //[CustomAuthorize(RoleEnum.Admin, RoleEnum.Manager)]
        [HttpPost(ApiEndPointConstant.Account.StaffEndPoint)]
        public async Task<IActionResult> CreateNewStaff(CreateNewStaffRequest createNewUserRequest)
        {
            var response = await _userService.CreateNewStaff(createNewUserRequest);
            return Ok(response);
        }

        //[Authorize]
        //[CustomAuthorize(RoleEnum.Admin, RoleEnum.Manager)]
        [HttpGet(ApiEndPointConstant.Account.AccountsEndPoint)]
        public async Task<IActionResult> GetAllUsers([FromQuery] UserFilter filter, [FromQuery] PagingModel pagingModel)
        {
            var response = await _userService.GetAllUsers(filter, pagingModel);

            return Ok(response);
        }
        //[HttpGet(ApiEndPointConstant.User.GetTaskOfStaff)]
        //public async Task<IActionResult> GetAllUsers()
        //{
        //    var response = await _userService.GetStaffTaskStatusesByRole();
        //    return Ok(response);
        //}

        [HttpGet(ApiEndPointConstant.Account.AccountEndPoint)]
        public async Task<IActionResult> GetUserById(Guid id)
        {
            var response = await _userService.GetUserById(id);
            return Ok(response);
        }
        [HttpPut(ApiEndPointConstant.Account.AccountEndPoint)]
        public async Task<IActionResult> UpdateUserInfor(Guid id, UpdateUserInforRequest updateRequest)
        {
            var isSuccessful = await _userService.UpdateUserInfor(id, updateRequest);
            if (!isSuccessful) return Ok(MessageConstant.User.UpdateStatusFailedMessage);
            return Ok(MessageConstant.User.UpdateStatusSuccessMessage);
        }
        [HttpDelete(ApiEndPointConstant.Account.AccountEndPoint)]
        public async Task<IActionResult> RemoveUserStatus(Guid id)
        {
            var isSuccessful = await _userService.RemoveUserStatus(id);
            if (!isSuccessful) return Ok(MessageConstant.User.UpdateStatusFailedMessage);
            return Ok(MessageConstant.User.UpdateStatusSuccessMessage);
        }
        //[HttpPost(ApiEndPointConstant.Account.AccountToRankEndPoint)]
        //public async Task<IActionResult> AddrankForAccount(Guid id, [FromBody] Guid rankId)
        //{
        //    var response = await _userService.AddrankForAccount(id, rankId);
        //    return Ok(response);
        //}

        [HttpPost(ApiEndPointConstant.Account.AccountEndPointChangePassword)]
        public async Task<IActionResult> ChangePassword(Guid id, [FromBody] ChangePasswordRequest changePasswordRequest)
        {

            var isSuccessful = await _userService.ChangePassword(id, changePasswordRequest);
            if (!isSuccessful) return Ok(new { Message = MessageConstant.User.ChangePasswordToFailed });
            return Ok(new { Message = MessageConstant.User.ChangePasswordToSuccess });

        }
        [HttpGet(ApiEndPointConstant.Account.GetTaskOfStaff)]
        public async Task<IActionResult> GetStaffTaskStatusesByRole(DateTime targetDate)
        {
            var response = await _userService.GetStaffTaskStatusesByRole(targetDate);
            return Ok(response);
        }
    }
}