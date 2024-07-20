using LMS.Api.Helpers.Interfaces;
using LMS.Services;
using LMS.Services.Common;
using LMS.Services.Interfaces;
using LMS.Services.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace LMS.Api.Controllers
{
    [Authorize]
    [Route("api/leave-type")]
    [ApiController]
    public class LeaveTypeController(ILeaveTypeService leaveTypeService, IApiResponseHelper apiResponseHelper) : ControllerBase
    {
        private readonly ILeaveTypeService _leaveTypeService = leaveTypeService;
        private readonly IApiResponseHelper _apiResponseHelper = apiResponseHelper;

        [HttpPost]
        public async Task<IActionResult> CreateNewLeaveType(LeaveTypeViewModel model)
        {
            try
            {
                var result = await _leaveTypeService.CreateLeaveType(model);
                return Ok(_apiResponseHelper.GenerateApiResponse(true, "Leave type created successfully!", result));
            }
            catch (Exception)
            {
                throw;
            }
        }
        [HttpPatch]
        public async Task<IActionResult> UpdateLeaveType(LeaveTypeViewModel model)
        {
            try
            {
                var result = await _leaveTypeService.UpdateLeaveType(model);
                return Ok(_apiResponseHelper.GenerateApiResponse(true, "Leave type updated successfully!", result));
            }
            catch (Exception)
            {
                throw;
            }
        }
        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> DeleteLeaveTypeById(int id)
        {
            try
            {
                var result = await _leaveTypeService.DeleteLeaveTypeById(id);
                return Ok(_apiResponseHelper.GenerateApiResponse(true, "Leave type deleted successfully!", result));
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet]
        [Route("get-leave-types")]
        public async Task<IActionResult> GetAllLeaveTypes()
        {
            try
            {
                var result = await _leaveTypeService.GetAllLeaveTypes();
                return Ok(_apiResponseHelper.GenerateApiResponse(true, result));
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost]
        [Route("get-leave-types/ssr")]
        public async Task<IActionResult> GetAllLeaveTypesSsr(DataTableConfiguration dataTableConfiguration)
        {
            try
            {
                var result = await _leaveTypeService.GetAllLeaveTypesSsr(dataTableConfiguration);
                return Ok(_apiResponseHelper.GenerateApiResponse(true, result));
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetLeaveTypeById(int id)
        {
            try
            {
                var result = await _leaveTypeService.GetLeaveTypeById(id);
                return Ok(_apiResponseHelper.GenerateApiResponse(true, result));
            }
            catch (Exception)
            {
                throw;
            }
        }
        [HttpGet]
        [Route("get-leave-type-by-name/{typeName}")]
        public async Task<IActionResult> GetLeavesTypeByName(string typeName)
        {
            try
            {
                var result = await _leaveTypeService.GetLeavesTypeByName(typeName);
                return Ok(_apiResponseHelper.GenerateApiResponse(true, result));
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
