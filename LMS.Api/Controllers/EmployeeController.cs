using LMS.Data.Enum;
using LMS.Services.Interfaces;
using LMS.Services.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace LMS.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    public class EmployeeController : ControllerBase
    {
        private readonly IEmployeeService _employeeService;

        public EmployeeController(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllEmployees(int page, int size)
        {
            return Ok(await _employeeService.GetAllEmployees(page, size));
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetAllEmployeeById(int id)
        {
            return Ok(await _employeeService.GetAllEmployeeById(id));
        }

        [HttpGet]
        [Route("get-employees-under-supervision/{id}")]
        public async Task<IActionResult> GetEmployeeUnderSupervision(int id)
        {
            return Ok(await _employeeService.GetEmployeesUnderSupervision(id));
        }

        [HttpGet]
        [Route("get-employee-by-full-name/{fullName}")]
        public async Task<IActionResult> GetAllEmployeeByFullName(string fullName)
        {
            return Ok(await _employeeService.GetEmployeeByFullName(fullName));
        }

        [HttpPost]
        public async Task<IActionResult> CreateNewEmployee(SignUpViewModel model)
        {
            return Ok(await _employeeService.CreateNewEmployee(model));
        }
        [HttpPatch]
        public async Task<IActionResult> UpdateEmployee(SystemUserViewModel model)
        {
            return Ok(await _employeeService.UpdateEmployee(model));
        }
        [HttpPatch]
        [Route("{id}")]
        public async Task<IActionResult> InactivateEmployee(int id)
        {
            return Ok(await _employeeService.InactivateEmployeeById(id));
        }
        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> DeleteEmployee(int id)
        {
            return Ok(await _employeeService.DeleteEmployeeById(id));
        }
    }
}
