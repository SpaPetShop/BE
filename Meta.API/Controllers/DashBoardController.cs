using DentalLabManagement.API.Controllers;
using Meta.BusinessTier.Constants;
using Meta.BusinessTier.Payload.User;
using Meta.BusinessTier.Payload;
using Meta.BusinessTier.Services.Implements;
using Meta.BusinessTier.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Meta.API.Controllers
{
    [ApiController]
    public class DashBoardController : BaseController<DashBoardController> 
    {
        readonly IDashBoardService _dashBoardService;

        public DashBoardController(ILogger<DashBoardController> logger, IDashBoardService dashBoardService) : base(logger)
        {
            _dashBoardService = dashBoardService;
        }
        [HttpGet(ApiEndPointConstant.DashBoard.DashBoardCountAccountEndpoint)]
        public async Task<IActionResult> CountAllAccount()
        {
            var response = await _dashBoardService.CountAllAccount();

            return Ok(response);
        }
        [HttpGet(ApiEndPointConstant.DashBoard.DashBoardCountOrderEndpoint)]
        public async Task<IActionResult> CountAllOrde()
        {
            var response = await _dashBoardService.CountAllOrde();

            return Ok(response);
        }
        [HttpGet(ApiEndPointConstant.DashBoard.DashBoardCountOrderInYearEndpoint)]
        public async Task<IActionResult> GetYearlyStatistics(int year)
        {
            var response = await _dashBoardService.GetYearlyStatistics(year);

            return Ok(response);
        }

    }
}
