using LMS.Data.Enum;
using LMS.Services.Common;
using LMS.Services.Constants;
using LMS.Services.Helpers.Interfaces;
using LMS.Services.Interfaces;
using LMS.Services.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Drawing;
using System.Security.Claims;
using System.Security.Policy;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace LMS.Api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly IEmployeeService _employeeService;
        private readonly IApiResponseHelper _apiResponseHelper;

        public EmployeeController(IEmployeeService employeeService, IApiResponseHelper apiResponseHelper)
        {
            _employeeService = employeeService;
            _apiResponseHelper = apiResponseHelper;
        }

        [HttpGet]
        [Route("get-employees")]
        public async Task<IActionResult> GetAllEmployees()
        {
            try
            {
                var result = await _employeeService.GetAllEmployees();
                return Ok(_apiResponseHelper.GenerateApiResponse(true, result));
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost]
        [Route("get-employees-ssr")]
        public async Task<IActionResult> GetAllEmployeesSsr(DataTableConfiguration dataTableConfiguration)
        {
            try
            {
                var result = await _employeeService.GetAllEmployeesSsr(dataTableConfiguration);
                return Ok(_apiResponseHelper.GenerateApiResponse(true, result));
            } catch (Exception) {
                throw;
            }
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetEmployeeById(int id)
        {
            try
            {
                var result = await _employeeService.GetEmployeeById(id);
                return Ok(_apiResponseHelper.GenerateApiResponse(true, result));
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet]
        [Route("get-employees-under-supervision/{id}")]
        public async Task<IActionResult> GetEmployeeUnderSupervision(int id)
        {
            try
            {
                var result = await _employeeService.GetEmployeesUnderSupervision(id);
                return Ok(_apiResponseHelper.GenerateApiResponse(true, result));
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet]
        [Route("get-employee-by-full-name/{fullName}")]
        public async Task<IActionResult> GetAllEmployeeByFullName(string fullName)
        {
            try
            {
                var result = await _employeeService.GetEmployeeByFullName(fullName);
                return Ok(_apiResponseHelper.GenerateApiResponse(true, result));
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateNewEmployee(SignUpViewModel model)
        {
            try
            {
                var result = await _employeeService.CreateNewEmployee(model);
                return Ok(_apiResponseHelper.GenerateApiResponse(true, "Employee created successfully!", result));
            }
            catch (Exception)
            {
                throw;
            }
        }
        [HttpPatch]
        public async Task<IActionResult> UpdateEmployee(SystemUserViewModel model)
        {
            try
            {
                var result = await _employeeService.UpdateEmployee(model);
                return Ok(_apiResponseHelper.GenerateApiResponse(true, "Employee updated successfully!", result));
            }
            catch (Exception)
            {
                throw;
            }
        }
        [HttpGet]
        [Route("inactivate-system-user/{id}")]
        public async Task<IActionResult> InactivateEmployee(int id)
        {
            try
            {
                var result = await _employeeService.InactivateEmployeeById(id);
                return Ok(_apiResponseHelper.GenerateApiResponse(true, "Employee inactivated successfully!", result));
            }
            catch (Exception)
            {
                throw;
            }
        }
        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> DeleteEmployee(int id)
        {
            try
            {
                var result = await _employeeService.DeleteEmployeeById(id);
                return Ok(_apiResponseHelper.GenerateApiResponse(true, "Employee deleted successfully!", result));
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
