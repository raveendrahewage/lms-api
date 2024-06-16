using LMS.Services;
using LMS.Services.Helpers.Interfaces;
using LMS.Services.Interfaces;
using LMS.Services.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Drawing;
using System.Security.Policy;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace LMS.Api.Controllers
{
    [Authorize]
    [Route("api/leave")]
    [ApiController]
    public class LeaveController : ControllerBase
    {
        private readonly ILeaveService _leaveService;
        private readonly IApiResponseHelper _apiResponseHelper;
        public LeaveController(ILeaveService leaveService, IApiResponseHelper apiResponseHelper)
        {
            _leaveService = leaveService;
            _apiResponseHelper = apiResponseHelper;
        }
        [HttpPost]
        public async Task<IActionResult> CreateNewLeave(LeaveViewModel model)
        {
            try
            {
                var result = await _leaveService.CreateLeave(model);
                return Ok(_apiResponseHelper.GenerateApiResponse(true, "Leave created successfully!", result));
            }
            catch (Exception)
            {
                throw;
            }
        }
        [HttpPatch]
        public async Task<IActionResult> UpdateLeave(LeaveViewModel model)
        {
            try
            {
                var result = await _leaveService.UpdateLeave(model);
                return Ok(_apiResponseHelper.GenerateApiResponse(true, "Leave updated successfully!", result));
            }
            catch (Exception)
            {
                throw;
            }
        }
        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> DeleteLeaveById(int id)
        {
            try
            {
                var result = await _leaveService.DeleteLeaveById(id);
                return Ok(_apiResponseHelper.GenerateApiResponse(true, "Leave deleted successfully!", result));
            }
            catch (Exception)
            {
                throw;
            }
        }
        [HttpGet]
        public async Task<IActionResult> GetAllLeaves(int? page, int? size)
        {
            try
            {
                var result = await _leaveService.GetAllLeaves(page, size);
                return Ok(_apiResponseHelper.GenerateApiResponse(true, result));
            }
            catch (Exception)
            {
                throw;
            }
        }
        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetLeaveById(int id)
        {
            try
            {
                var result = await _leaveService.GetLeaveById(id);
                return Ok(_apiResponseHelper.GenerateApiResponse(true, result));
            }
            catch (Exception)
            {
                throw;
            }
        }
        [HttpGet]
        [Route("get-leaves-between")]
        public async Task<IActionResult> GetLeavesBetween(DateTime startDate, DateTime endDate)
        {
            try
            {
                var result = await _leaveService.GetLeavesBetween(startDate, endDate);
                return Ok(_apiResponseHelper.GenerateApiResponse(true, result));
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
