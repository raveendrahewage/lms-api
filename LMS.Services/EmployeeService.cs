using AutoMapper;
using LMS.Data;
using LMS.Data.Enum;
using LMS.Data.Models;
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
        private readonly IHttpContextAccessor _httpContextAccessor;

        public EmployeeService(
            ApplicationDbContext appDbContext,
            RoleManager<SystemRole> roleManager,
            SignInManager<SystemUser> signInManager,
            UserManager<SystemUser> userManager,
            IConfiguration configuration,
            IMapper mapper,
            IHttpContextAccessor httpContextAccessor
        )
        {
            _appDbContext = appDbContext;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _userManager = userManager;
            _configuration = configuration;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<List<SystemUserViewModel>> GetAllEmployees(int? page, int? size)
        {
            try
            {
                List<SystemUser> systemUsers = new List<SystemUser>();
                if (page.HasValue && size.HasValue && page > 0 && size > 0)
                {
                    systemUsers= await _appDbContext.SystemUsers
                        .Skip((page.Value - 1) * size.Value)
                        .Take(size.Value)
                        .ToListAsync();
                }
                else
                {
                    systemUsers= await _appDbContext.SystemUsers.ToListAsync();
                }
                return _mapper.Map<List<SystemUserViewModel>>(systemUsers);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<SystemUserViewModel> GetEmployeeById(int id)
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
                    return _mapper.Map<SystemUserViewModel>(systemUser);
                }
                throw new Exception("Employee not found!");
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<SystemUserViewModel> GetEmployeeByFullName(string fullName)
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
                    return _mapper.Map<SystemUserViewModel>(systemUser);
                }
                throw new Exception("Employee not found!");
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<SystemUserViewModel> CreateNewEmployee(SignUpViewModel model)
        {
            try
            {
                var signUpResult = await CreateSystemUserAsync(model);
                if (signUpResult)
                {
                    var systemUser = await _userManager.FindByEmailAsync(model.Email);
                    return _mapper.Map<SystemUserViewModel>(systemUser);
                }
                throw new Exception("Employee was not created! Something went wrong.");
            }
            catch (Exception)
            {
                throw;
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

        public async Task<SystemUserViewModel> UpdateEmployee(SystemUserViewModel model)
        {
            using (var transaction = _appDbContext.Database.BeginTransaction())
            {
                try
                {
                    var systemUser = await _appDbContext.SystemUsers
                        .FirstOrDefaultAsync(x => x.Id == model.Id);
                    if (systemUser is not null)
                    {
                        systemUser.FirstName = model.FirstName;
                        systemUser.LastName = model.LastName;
                        systemUser.PhoneNumber = model.PhoneNumber;
                        systemUser.Email = model.Email;
                        systemUser.UserName = model.Email;
                        systemUser.NormalizedEmail = model.Email;
                        systemUser.NormalizedUserName = model.Email;
                        systemUser.SupervisorId = model.SupervisorId.HasValue && model.SupervisorId.Value > 0 ? model.SupervisorId: null;
                        var result = await _userManager.UpdateAsync(systemUser);
                        if (result.Succeeded)
                        {
                            await _appDbContext.SaveChangesAsync();
                            transaction.Commit();
                            return _mapper.Map<SystemUserViewModel>(systemUser);
                        }
                        throw new Exception("Updating failed!. Something went wrong.");
                    }
                    throw new Exception("Employee not found!");
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }
        public async Task<List<SystemUserViewModel>> GetEmployeesUnderSupervision(int id)
        {
            try
            {
                var systemUser = await _appDbContext.SystemUsers
                    .Include(x => x.EmployeesUnderSupervision)
                        .ThenInclude(x => x.Role)
                    .FirstOrDefaultAsync(x => x.Id == id);
                if(systemUser is not null)
                {
                    return _mapper.Map<List<SystemUserViewModel>>(systemUser?.EmployeesUnderSupervision);
                }
                throw new Exception("Employee not found!");
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<SystemUserViewModel> DeleteEmployeeById(int id)
        {
            try
            {
                var employee = await _appDbContext.SystemUsers
                    .FirstOrDefaultAsync(x => x.Id == id);
                if (employee is not null)
                {
                    employee.Status = DataRecordStatus.Deleted;
                    _appDbContext.SystemUsers.Update(employee);
                    await _appDbContext.SaveChangesAsync();
                   return _mapper.Map<SystemUserViewModel>(employee);
                }
                throw new Exception("Employee not found!");
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<SystemUserViewModel> InactivateEmployeeById(int id)
        {
            try
            {
                var employee = await _appDbContext.SystemUsers
                    .FirstOrDefaultAsync(x => x.Id == id);
                if (employee is not null)
                {
                    employee.Status = DataRecordStatus.Inactive;
                    _appDbContext.SystemUsers.Update(employee);
                    await _appDbContext.SaveChangesAsync();
                    return _mapper.Map<SystemUserViewModel>(employee);
                }
                throw new Exception("Employee not found!");
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
