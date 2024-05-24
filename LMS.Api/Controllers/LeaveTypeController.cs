using LMS.Services;
using LMS.Services.Interfaces;
using LMS.Services.ViewModels;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace LMS.Api.Controllers
{
    [Route("api/leave-type")]
    [ApiController]
    public class LeaveTypeController : ControllerBase
    {
        private readonly ILeaveTypeService _leaveTypeService;
        public LeaveTypeController(ILeaveTypeService leaveTypeService)
        {
            _leaveTypeService = leaveTypeService;
        }
        [HttpPost]
        public async Task<IActionResult> CreateNewLeaveType(LeaveTypeViewModel model)
        {
            return Ok(await _leaveTypeService.CreateLeaveType(model));
        }
        [HttpPatch]
        public async Task<IActionResult> UpdateLeaveType(LeaveTypeViewModel model)
        {
            return Ok(await _leaveTypeService.UpdateLeaveType(model));
        }
        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> DeleteLeaveTypeById(int id)
        {
            return Ok(await _leaveTypeService.DeleteLeaveTypeById(id));
        }
        [HttpGet]
        public async Task<IActionResult> GetLeaveTypeById()
        {
            return Ok(await _leaveTypeService.GetAllLeaveTypes());
        }
        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetLeaveTypeById(int id)
        {
            return Ok(await _leaveTypeService.GetLeaveTypeById(id));
        }
        [HttpGet]
        [Route("get-leave-type-by-name/{typeName}")]
        public async Task<IActionResult> GetLeavesTypeByName(string typeName)
        {
            return Ok(await _leaveTypeService.GetLeavesTypeByName(typeName));
        }
    }
}
