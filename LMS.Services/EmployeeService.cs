using AutoMapper;
using LMS.Data;
using LMS.Data.Enum;
using LMS.Data.Models;
using LMS.Services.Common;
using LMS.Services.Constants;
using LMS.Services.Helpers;
using LMS.Services.Interfaces;
using LMS.Services.Responses;
using LMS.Services.ViewModels;
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
    public class EmployeeService(
        ApplicationDbContext appDbContext,
        UserManager<SystemUser> userManager,
        IMapper mapper,
        IAccountService accountService
        ) : IEmployeeService
    {
        private readonly ApplicationDbContext _appDbContext = appDbContext;
        private readonly UserManager<SystemUser> _userManager = userManager;
        private readonly IMapper _mapper = mapper;
        private readonly IAccountService _accountService = accountService;

        public async Task<List<SystemUserViewModel>> GetAllEmployees()
        {
            try
            {
                var systemUsers = await _appDbContext.SystemUsers
                        .Include(x => x.Role)
                        .Include(x => x.Leaves)
                        .Include(x => x.ReviewedLeaves)
                        .Include(x => x.Supervisor)
                        .Include(x => x.EmployeesUnderSupervision)
                        .ToListAsync();
                return _mapper.Map<List<SystemUser>, List<SystemUserViewModel>>(systemUsers);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<DataTableResult<SystemUserListItemViewModel>> GetAllEmployeesSsr(DataTableConfiguration dataTableConfiguration)
        {
            try
            {
                var systemUsers = await _appDbContext.SystemUsers
                        .IgnoreQueryFilters()
                        .Include(x => x.Role)
                        .Include(x => x.Supervisor)
                        .Where(x => x.Status == DataRecordStatus.Active || x.Status == DataRecordStatus.Inactive)
                        .ToListAsync();
                var systemUserListItemViewModels = _mapper.Map<List<SystemUser>, List<SystemUserListItemViewModel>>(systemUsers);
                return DataTableResultHandler<SystemUserListItemViewModel>.ResultToSsr(systemUserListItemViewModels, dataTableConfiguration, DataTableConfigurationOptions.All);
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
                    .Include(x => x.Supervisor) 
                    .Include(x => x.ReviewedLeaves)
                    .Include(x => x.EmployeesUnderSupervision)
                    .Include(x => x.LeaveAvailabilities.Where(x => x.Year == DateTime.UtcNow.Year))
                    .FirstOrDefaultAsync(x => x.Id == id);
                if(systemUser is not null)
                {
                    return _mapper.Map<SystemUser, SystemUserViewModel>(systemUser);
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
                    .Include(x => x.Supervisor)
                    .Include(x => x.ReviewedLeaves)
                    .Include(x => x.EmployeesUnderSupervision)
                    .FirstOrDefaultAsync(x => (x.FirstName.ToLower().Trim() + " " + x.LastName.ToLower().Trim()).Equals(fullName.Trim().ToLower()));
                if(systemUser is not null)
                {
                    return _mapper.Map<SystemUser, SystemUserViewModel>(systemUser);
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
            var loggedInUser = _accountService.GetCurrentLoggedInUser();
            if (loggedInUser is null || !(loggedInUser.Role?.Name ?? string.Empty).Equals(SysRole.Admin))
                throw new Exception("You don't have permission to make changes to this employee.");

            using var transaction = await _appDbContext.Database.BeginTransactionAsync();
            try
            {
                var signUpResult = await CreateSystemUserAsync(model);
                var systemUser = await _userManager.FindByEmailAsync(model.Email);
                if (signUpResult && systemUser is not null)
                {
                    await UpdateEmployeesUnderSupervision(model.EmployeesUnderSupervision, systemUser.Id);
                    await UpdateOrCreateLeaveAvailabulities(model.LeaveAvailabilities);
                    await _appDbContext.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return _mapper.Map<SystemUser, SystemUserViewModel>(systemUser);
                }
                throw new Exception("Employee was not created! Something went wrong.");
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
        public async Task<bool> CreateSystemUserAsync(SignUpViewModel model)
        {
            try
            {
                var newSystemUser = _mapper.Map<SignUpViewModel, SystemUser>(model);
                newSystemUser.EmployeesUnderSupervision = [];
                newSystemUser.EmailConfirmed = true;
                newSystemUser.CreatedBy = _accountService.GetCurrentLoggedInUserId();
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
            var loggedInUser = _accountService.GetCurrentLoggedInUser();
            if (loggedInUser is null || !(loggedInUser.Role?.Name ?? string.Empty).Equals(SysRole.Admin) && loggedInUser.Id != model.Id)
                throw new Exception("You don't have permission to make changes to this employee.");

            using var transaction = await _appDbContext.Database.BeginTransactionAsync();
            try
            {
                var systemUser = await _appDbContext.SystemUsers
                    .Include(x => x.Role)
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
                    systemUser.SupervisorId = model.SupervisorId;
                    systemUser.Status = model.Status;
                    systemUser.ModifiedBy = _accountService.GetCurrentLoggedInUserId();
                    systemUser.ModifiedDate = DateTime.UtcNow;
                    var result = await _userManager.UpdateAsync(systemUser);
                    if (result.Succeeded)
                    {
                        if((loggedInUser.Role?.Name ?? string.Empty).Equals(SysRole.Admin))
                        {
                            await UpdateEmployeesUnderSupervision(model.EmployeesUnderSupervision, systemUser.Id);
                            await UpdateOrCreateLeaveAvailabulities(model.LeaveAvailabilities);
                        }

                        await _appDbContext.SaveChangesAsync();
                        await transaction.CommitAsync();
                        systemUser = await _appDbContext.SystemUsers
                            .Include(x => x.Role)
                            .FirstAsync(x => x.Id == model.Id);
                        return _mapper.Map<SystemUser, SystemUserViewModel>(systemUser);
                    }
                    throw new Exception("Updating failed!. Something went wrong.");
                }
                throw new Exception("Employee not found!");
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
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
                    return _mapper.Map<List<SystemUser>, List<SystemUserViewModel>>(systemUser.EmployeesUnderSupervision);
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
                    employee.DeletedBy = _accountService.GetCurrentLoggedInUserId();
                    employee.DeletedDate = DateTime.UtcNow;
                    _appDbContext.SystemUsers.Update(employee);
                    await _appDbContext.SaveChangesAsync();
                   return _mapper.Map<SystemUser, SystemUserViewModel>(employee);
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
                    employee.ModifiedBy = _accountService.GetCurrentLoggedInUserId();
                    employee.ModifiedDate = DateTime.UtcNow;
                    _appDbContext.SystemUsers.Update(employee);
                    await _appDbContext.SaveChangesAsync();
                    return _mapper.Map<SystemUser, SystemUserViewModel>(employee);
                }
                throw new Exception("Employee not found!");
            }
            catch (Exception)
            {
                throw;
            }
        }

        private async Task UpdateEmployeesUnderSupervision(IEnumerable<SystemUserViewModel> employeesUnderSupervision, int supervisorId)
        {
            var employeeIds = employeesUnderSupervision.Select(e => e.Id).ToList();
            var employees = await _appDbContext.SystemUsers
                .Where(x => employeeIds.Contains(x.Id))
                .ToListAsync();

            foreach (var employee in employees)
            {
                employee.SupervisorId = supervisorId;
                employee.ModifiedBy = _accountService.GetCurrentLoggedInUserId();
                employee.ModifiedDate = DateTime.UtcNow;
            }

            _appDbContext.UpdateRange(employees);
        }
        private async Task UpdateOrCreateLeaveAvailabulities(IEnumerable<LeaveAvailabilityViewModel> leaveAvailabilities)
        {
            foreach (var leaveAvailabilityViewModel in leaveAvailabilities)
            {
                var leaveAvailability = await _appDbContext.LeaveAvailabilities
                    .FirstOrDefaultAsync(x => x.Year == DateTime.UtcNow.Year && x.SystemUserId == leaveAvailabilityViewModel.SystemUserId && x.LeaveTypeId == leaveAvailabilityViewModel.LeaveTypeId);
                if (leaveAvailability is not null)
                {
                    leaveAvailability.LeaveCount = leaveAvailabilityViewModel.LeaveCount;
                    leaveAvailability.BalanceCount = leaveAvailabilityViewModel.LeaveCount - leaveAvailability.BookedCount;
                    leaveAvailability.ModifiedBy = _accountService.GetCurrentLoggedInUserId();
                    leaveAvailability.ModifiedDate = DateTime.UtcNow;

                    _appDbContext.LeaveAvailabilities.Update(leaveAvailability);
                }
                else
                {
                    var dbLeaveAvailability = _mapper.Map<LeaveAvailabilityViewModel, LeaveAvailability>(leaveAvailabilityViewModel);
                    await _appDbContext.LeaveAvailabilities.AddAsync(dbLeaveAvailability);
                }
            }
        }
    }
}
