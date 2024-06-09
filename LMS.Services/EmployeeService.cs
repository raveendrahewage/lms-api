using AutoMapper;
using LMS.Data;
using LMS.Data.Enum;
using LMS.Data.Models;
using LMS.Services.Helpers.Interfaces;
using LMS.Services.Interfaces;
using LMS.Services.Responses;
using LMS.Services.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly ApplicationDbContext _appDbContext;
        private readonly RoleManager<SystemRole> _roleManager;
        private readonly SignInManager<SystemUser> _signInManager;
        private readonly UserManager<SystemUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;
        private readonly IApiResponseHelper _apiResponseHelper;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public EmployeeService(
            ApplicationDbContext appDbContext,
            RoleManager<SystemRole> roleManager,
            SignInManager<SystemUser> signInManager,
            UserManager<SystemUser> userManager,
            IConfiguration configuration,
            IMapper mapper,
            IApiResponseHelper apiResponseHelper,
            IHttpContextAccessor httpContextAccessor
        )
        {
            _appDbContext = appDbContext;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _userManager = userManager;
            _configuration = configuration;
            _mapper = mapper;
            _apiResponseHelper = apiResponseHelper;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<ApiResponse<List<SystemUserViewModel>>> GetAllEmployees(int page, int size)
        {
            try
            {
                var systemUsers = await _appDbContext.SystemUsers
                    .Skip((page - 1) * size).Take(size)
                    .ToListAsync();
                var employees = _mapper.Map<List<SystemUserViewModel>>(systemUsers);
                return _apiResponseHelper.GenerateApiResponse<List<SystemUserViewModel>>(true, employees);
            }
            catch (Exception ex)
            {
                return _apiResponseHelper.GenerateApiResponse<List<SystemUserViewModel>>(false, ex.Message);
            }
        }
        public async Task<ApiResponse<SystemUserViewModel>> GetEmployeeById(int id)
        {
            try
            {
                var systemUser = await _appDbContext.SystemUsers
                    .Include(x => x.Role)
                    .Include(x => x.Leaves)
                    .Include(x => x.ReviewedLeaves)
                    .Include(x => x.EmployeesUnderSupervision)
                    .FirstOrDefaultAsync(x => x.Id == id);
                if(systemUser is not null)
                {
                    var employee = _mapper.Map<SystemUserViewModel>(systemUser);
                    return _apiResponseHelper.GenerateApiResponse<SystemUserViewModel>(true, employee);
                }
                return _apiResponseHelper.GenerateApiResponse<SystemUserViewModel>(false, "Employee not found!");
            }
            catch (Exception ex)
            {
                return _apiResponseHelper.GenerateApiResponse<SystemUserViewModel>(false, ex.Message);
            }
        }
        public async Task<ApiResponse<SystemUserViewModel>> GetEmployeeByFullName(string fullName)
        {
            try
            {
                var systemUser = await _appDbContext.SystemUsers
                    .Include(x => x.Role)
                    .Include(x => x.Leaves)
                    .Include(x => x.ReviewedLeaves)
                    .Include(x => x.EmployeesUnderSupervision)
                    .FirstOrDefaultAsync(x => (x.FirstName.ToLower().Trim() + " " + x.LastName.ToLower().Trim()).Equals(fullName.Trim().ToLower()));
                if(systemUser is not null)
                {
                    var employee = _mapper.Map<SystemUserViewModel>(systemUser);
                    return _apiResponseHelper.GenerateApiResponse<SystemUserViewModel>(true, employee);
                }
                return _apiResponseHelper.GenerateApiResponse<SystemUserViewModel>(false, "Employee not found!");
            }
            catch (Exception ex)
            {
                return _apiResponseHelper.GenerateApiResponse<SystemUserViewModel>(false, ex.Message);
            }
        }
        public async Task<ApiResponse<SystemUserViewModel>> CreateNewEmployee(SignUpViewModel model)
        {
            try
            {
                var signUpResult = await CreateSystemUserAsync(model);
                if (signUpResult)
                {
                    var systemUser = await _userManager.FindByEmailAsync(model.Email);
                    var systemUserViewModel = _mapper.Map<SystemUserViewModel>(systemUser);
                    return new ApiResponse<SystemUserViewModel>(
                        signUpResult,
                        "Employee created successfully!",
                        systemUserViewModel
                    );
                }
                return _apiResponseHelper.GenerateApiResponse<SystemUserViewModel>(signUpResult, "Employee was not created! Something went wrong.");
            }
            catch (Exception ex)
            {
                return _apiResponseHelper.GenerateApiResponse<SystemUserViewModel>(false, ex.Message);
            }
        }
        public async Task<bool> CreateSystemUserAsync(SignUpViewModel model)
        {
            try
            {
                var newSystemUser = _mapper.Map<SignUpViewModel, SystemUser>(model);
                newSystemUser.SupervisorId = model.SupervisorId is not null && model.SupervisorId > 0 ? model.SupervisorId : null;
                var signUpResult = await _userManager.CreateAsync(newSystemUser, model.Password);
                if (signUpResult != null && signUpResult.Succeeded) return true;
                return false;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public async Task<ApiResponse<SystemUserViewModel>> UpdateEmployee(SystemUserViewModel model)
        {
            try
            {
                var systemUserToBeUpdated = await _appDbContext.SystemUsers
                    .FirstOrDefaultAsync(x => x.Id == model.Id);
                if (systemUserToBeUpdated is not null)
                {
                    systemUserToBeUpdated.FirstName = model.FirstName;
                    systemUserToBeUpdated.LastName = model.LastName;
                    systemUserToBeUpdated.PhoneNumber = model.PhoneNumber;
                    systemUserToBeUpdated.RoleId = model.RoleId;
                    systemUserToBeUpdated.SupervisorId = model.SupervisorId;
                    _appDbContext.SystemUsers.Update(systemUserToBeUpdated);
                    await _appDbContext.SaveChangesAsync();
                    return _apiResponseHelper.GenerateApiResponse<SystemUserViewModel>(true, "Employee updated successfully!", model);
                }
                return _apiResponseHelper.GenerateApiResponse<SystemUserViewModel>(false, "Employee not found!");
            }
            catch (Exception ex)
            {
                return _apiResponseHelper.GenerateApiResponse<SystemUserViewModel>(false, ex.Message);
            }
        }
        public async Task<ApiResponse<List<SystemUserViewModel>>> GetEmployeesUnderSupervision(int id)
        {
            try
            {
                var systemUser = await _appDbContext.SystemUsers
                    .Include(x => x.EmployeesUnderSupervision)
                        .ThenInclude(x => x.Role)
                    .FirstOrDefaultAsync(x => x.Id == id);
                if(systemUser is not null)
                {
                    var employeesUnderSupervisionViewModel = _mapper.Map<List<SystemUserViewModel>>(systemUser?.EmployeesUnderSupervision);
                    return _apiResponseHelper.GenerateApiResponse<List<SystemUserViewModel>>(true, employeesUnderSupervisionViewModel);
                }
                return _apiResponseHelper.GenerateApiResponse<List<SystemUserViewModel>>(false, "Employee not found!");
            }
            catch (Exception ex)
            {
                return _apiResponseHelper.GenerateApiResponse<List<SystemUserViewModel>>(false, ex.Message);
            }
        }
        public async Task<ApiResponse<SystemUserViewModel>> DeleteEmployeeById(int id)
        {
            try
            {
                var employee = await _appDbContext.SystemUsers
                    .FirstOrDefaultAsync(x => x.Id == id);
                if (employee is not null)
                {
                    employee.Status = DataRecordStatus.Deleted;
                    var employeeViewModel = _mapper.Map<SystemUserViewModel>(employee);
                    _appDbContext.SaveChanges();
                    return _apiResponseHelper.GenerateApiResponse<SystemUserViewModel>(true, "Employee deleted successfully!", employeeViewModel);
                }
                return _apiResponseHelper.GenerateApiResponse<SystemUserViewModel>(false, "Employee not found!");
            }
            catch (Exception ex)
            {
                return _apiResponseHelper.GenerateApiResponse<SystemUserViewModel>(false, ex.Message);
            }
        }
        public async Task<ApiResponse<SystemUserViewModel>> InactivateEmployeeById(int id)
        {
            try
            {
                var employee = await _appDbContext.SystemUsers
                    .FirstOrDefaultAsync(x => x.Id == id);
                if (employee is not null)
                {
                    employee.Status = DataRecordStatus.Inactive;
                    var employeeViewModel = _mapper.Map<SystemUserViewModel>(employee);
                    _appDbContext.SaveChanges();
                    return _apiResponseHelper.GenerateApiResponse<SystemUserViewModel>(true, "Employee inactivated successfully!", employeeViewModel);
                }
                return _apiResponseHelper.GenerateApiResponse<SystemUserViewModel>(false, "Employee not found!");
            }
            catch (Exception ex)
            {
                return _apiResponseHelper.GenerateApiResponse<SystemUserViewModel>(false, ex.Message);
            }
        }
    }
}
