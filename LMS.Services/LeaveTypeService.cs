using AutoMapper;
using LMS.Data.Models;
using LMS.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LMS.Services.ViewModels;
using LMS.Services.Responses;
using Microsoft.EntityFrameworkCore;
using LMS.Data.Enum;
using LMS.Services.Interfaces;
using LMS.Services.Common;
using LMS.Services.Constants;
using LMS.Services.Helpers;

namespace LMS.Services
{
    public class LeaveTypeService(
        ApplicationDbContext appDbContext,
        IMapper mapper,
        IAccountService accountService
        ) : ILeaveTypeService
    {
        private readonly ApplicationDbContext _appDbContext = appDbContext;
        private readonly IMapper _mapper = mapper;
        private readonly IAccountService _accountService = accountService;

        public async Task<LeaveTypeViewModel> CreateLeaveType(LeaveTypeViewModel model)
        {
            try
            {
                var leaveTypeToBeCreated = _mapper.Map<LeaveTypeViewModel, LeaveType>(model);
                leaveTypeToBeCreated.CreatedBy = _accountService.GetCurrentLoggedInUserId();
                var result = await _appDbContext.LeaveTypes.AddAsync(leaveTypeToBeCreated);
                _appDbContext.SaveChanges();
                return model;
            } catch (Exception)
            {
                throw;
            }
        }
        public async Task<LeaveTypeViewModel> UpdateLeaveType(LeaveTypeViewModel model)
        {
            try
            {
                var leaveTypeToBeUpdated = await _appDbContext.LeaveTypes
                    .FirstOrDefaultAsync(x => x.Id == model.Id);
                if(leaveTypeToBeUpdated is not null)
                {
                    leaveTypeToBeUpdated.Name = model.Name;
                    leaveTypeToBeUpdated.DefaultLeaveCount = model.DefaultLeaveCount;
                    leaveTypeToBeUpdated.Status = model.Status;
                    leaveTypeToBeUpdated.ModifiedBy = _accountService.GetCurrentLoggedInUserId();
                    leaveTypeToBeUpdated.ModifiedDate = DateTime.UtcNow;
                    _appDbContext.LeaveTypes.Update(leaveTypeToBeUpdated);
                    await _appDbContext.SaveChangesAsync();
                    return _mapper.Map<LeaveType, LeaveTypeViewModel>(leaveTypeToBeUpdated);
                }
                throw new Exception("Leave type not found!");
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<List<LeaveTypeViewModel>> GetAllLeaveTypes()
        {
            try
            {
                var leaveTypes = await _appDbContext.LeaveTypes
                    .ToListAsync();
                return _mapper.Map<List<LeaveType>, List<LeaveTypeViewModel>>(leaveTypes);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<List<LeaveAvailabilityViewModel>> GetLeaveTypesForEmployee(int id)
        {
            try
            {
                var leaveAvailabilities= await _appDbContext.LeaveAvailabilities
                    .Include(x => x.LeaveType)
                    .Where(x => x.SystemUserId == id)
                    .ToListAsync();
                return _mapper.Map<List<LeaveAvailability>, List<LeaveAvailabilityViewModel>>(leaveAvailabilities);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<DataTableResult<LeaveTypeViewModel>> GetAllLeaveTypesSsr(DataTableConfiguration dataTableConfiguration)
        {
            try
            {
                var leaveTypes = await _appDbContext.LeaveTypes
                    .ToListAsync();
                var leaveTypeViewModels = _mapper.Map<List<LeaveType>, List<LeaveTypeViewModel>>(leaveTypes);
                return DataTableResultHandler<LeaveTypeViewModel>.ResultToSsr(leaveTypeViewModels, dataTableConfiguration, DataTableConfigurationOptions.All);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<List<LeaveTypeViewModel>> GetLeavesTypeByName(string typeName)
        {
            try
            {
                var leaveTypes = await _appDbContext.LeaveTypes
                    .Where(x => x.Name.Equals(typeName))
                    .ToListAsync();
                return _mapper.Map<List<LeaveType>, List<LeaveTypeViewModel>>(leaveTypes);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<LeaveTypeViewModel> GetLeaveTypeById(int id)
        {
            try
            {
                var leaveType = await _appDbContext.LeaveTypes
                    .FirstOrDefaultAsync(x => x.Id == id);
                if(leaveType is not null)
                {
                    return _mapper.Map<LeaveType, LeaveTypeViewModel>(leaveType);
                }
                throw new Exception("Leave type not found!");
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<LeaveTypeViewModel> DeleteLeaveTypeById(int id)
        {
            try
            {
                var leaveType = await _appDbContext.LeaveTypes
                    .FirstOrDefaultAsync(x => x.Id == id);
                if(leaveType is not null)
                {
                    leaveType.Status = DataRecordStatus.Deleted;
                    leaveType.DeletedBy = _accountService.GetCurrentLoggedInUserId();
                    leaveType.DeletedDate = DateTime.UtcNow;
                    _appDbContext.LeaveTypes.Update(leaveType);
                    await _appDbContext.SaveChangesAsync();
                    return _mapper.Map<LeaveType, LeaveTypeViewModel>(leaveType);
                }
                throw new Exception("Leave type not found!");
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
