using AutoMapper;
using Azure;
using LMS.Data;
using LMS.Data.Enum;
using LMS.Data.Models;
using LMS.Services.Common;
using LMS.Services.Constants;
using LMS.Services.Helpers;
using LMS.Services.Interfaces;
using LMS.Services.Responses;
using LMS.Services.ViewModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Services
{
    public class LeaveService : ILeaveService
    {
        private readonly ApplicationDbContext _appDbContext;
        private readonly IAccountService _accountService;
        private readonly IMapper _mapper;
        public LeaveService(
            ApplicationDbContext appDbContext,
            IAccountService accountService,
            IMapper mapper
        )
        {
            _appDbContext = appDbContext;
            _accountService = accountService;
            _mapper = mapper;
        }
        public async Task<LeaveViewModel> CreateLeave(LeaveViewModel model)
        {
            try
            {
                var isConflictingLeavesAvailable = _appDbContext.Leaves.Any(x =>
                    x.EmployeeId == _accountService.GetCurrentLoggedInUserId()
                    && x.FromDate >= model.FromDate
                    && x.ToDate <= model.ToDate);
                if(!isConflictingLeavesAvailable)
                {
                    var leaveToBeCreated = _mapper.Map<Leave>(model);
                    leaveToBeCreated.CreatedBy = _accountService.GetCurrentLoggedInUserId();
                    leaveToBeCreated.DateWiseLeaves.ForEach(l => l.CreatedBy = _accountService.GetCurrentLoggedInUserId());
                    var result = await _appDbContext.Leaves.AddAsync(leaveToBeCreated);
                    _appDbContext.SaveChanges();
                    return _mapper.Map<LeaveViewModel>(result.Entity);
                }
                throw new Exception("Selected dates conflict");
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<LeaveViewModel> UpdateLeave(LeaveViewModel model)
        {
            try
            {
                var leaveToBeUpdated = await _appDbContext.Leaves
                    .Include(x => x.Employee)
                    .Include(x => x.DateWiseLeaves)
                    .FirstOrDefaultAsync(x => x.Id == model.Id);
                if (leaveToBeUpdated is not null)
                {
                    var currentLoggedInUserId = _accountService.GetCurrentLoggedInUserId();
                    if (leaveToBeUpdated.EmployeeId == currentLoggedInUserId)
                    {
                        switch (model.LeaveStatus)
                        {
                            case LeaveStatus.Pending:
                                leaveToBeUpdated.LeaveTypeId = model.LeaveTypeId;
                                leaveToBeUpdated.Reason = model.Reason;
                                leaveToBeUpdated.LeaveStatus = LeaveStatus.Pending;
                                foreach (var dbDateWiseLeave in leaveToBeUpdated.DateWiseLeaves)
                                {
                                    var modelDateWiseLeave = model.DateWiseLeaves.First(x => x.Id == dbDateWiseLeave.Id);
                                    dbDateWiseLeave.LeaveDayType = modelDateWiseLeave.LeaveDayType;
                                    dbDateWiseLeave.LeaveHalfDayType = modelDateWiseLeave.LeaveHalfDayType;
                                    dbDateWiseLeave.LeaveQuarterDayType = modelDateWiseLeave.LeaveQuarterDayType;
                                    dbDateWiseLeave.ModifiedBy = currentLoggedInUserId;
                                    dbDateWiseLeave.ModifiedDate = DateTime.Now;
                                }
                                break;
                            case LeaveStatus.Canceled:
                                leaveToBeUpdated.LeaveStatus = LeaveStatus.Canceled;
                                break;
                        }
                        leaveToBeUpdated.ModifiedBy = currentLoggedInUserId;
                        leaveToBeUpdated.ModifiedDate = DateTime.Now;
                    }
                    else if (leaveToBeUpdated.Employee.SupervisorId == currentLoggedInUserId)
                    {
                        switch (model.LeaveStatus)
                        {
                            case LeaveStatus.Approved:
                                leaveToBeUpdated.LeaveStatus = LeaveStatus.Approved;
                                break;
                            case LeaveStatus.Denied:
                                leaveToBeUpdated.LeaveStatus = LeaveStatus.Denied;
                                leaveToBeUpdated.DeniedReason = model.DeniedReason;
                                break;
                        }
                        leaveToBeUpdated.ModifiedBy = currentLoggedInUserId;
                        leaveToBeUpdated.ModifiedDate = DateTime.Now;
                    }
                    else throw new Exception("You don't have permission to make changes.");
                    
                    var result = _appDbContext.Leaves.Update(leaveToBeUpdated);
                    await _appDbContext.SaveChangesAsync();
                    return _mapper.Map<LeaveViewModel>(result.Entity);
                }
                throw new Exception("Leave not found!");
            }
            catch (DbUpdateConcurrencyException ex)
            {
                var entry = ex.Entries.Single();
                var clientValues = (Leave)entry.Entity;
                var databaseEntry = entry.GetDatabaseValues();

                var databaseValues = (Leave)databaseEntry.ToObject();
                throw new Exception($"Leave is {Enum.GetName(typeof(LeaveStatus), databaseValues.LeaveStatus)}.");
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<List<LeaveViewModel>> GetAllLeavesByEmployeeId(int id)
        {
            try
            {
                var leaves = await _appDbContext.Leaves
                    .Where(x => x.EmployeeId == id || x.Employee.SupervisorId == id)
                    .ToListAsync();
                return _mapper.Map<List<LeaveViewModel>>(leaves);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<DataTableResult<LeaveListItemViewModel>> GetAllLeavesByEmployeeIdSsr(int id, LeaveFetchingMode leaveFetchingMode, DataTableConfiguration dataTableConfiguration)
        {
            try
            {
                List<Leave> leaves = [];
                switch (leaveFetchingMode)
                {
                    case LeaveFetchingMode.All:
                        leaves = await _appDbContext.Leaves
                        .Include(x => x.Employee)
                            .ThenInclude(x => x.Supervisor)
                        .Include(x => x.LeaveType)
                        .Where(x => x.EmployeeId == id || x.Employee.SupervisorId == id)
                        .ToListAsync();
                        break;
                    case LeaveFetchingMode.OnlyMine:
                        leaves = await _appDbContext.Leaves
                        .Include(x => x.Employee)
                            .ThenInclude(x => x.Supervisor)
                        .Include(x => x.LeaveType)
                        .Where(x => x.EmployeeId == id)
                        .ToListAsync();
                        break;
                    case LeaveFetchingMode.OnlyApprovals:
                        leaves = await _appDbContext.Leaves
                        .Include(x => x.Employee)
                            .ThenInclude(x => x.Supervisor)
                        .Include(x => x.LeaveType)
                        .Where(x => x.Employee.SupervisorId == id)
                        .ToListAsync();
                        break;
                    default:
                        leaves = await _appDbContext.Leaves
                        .Include(x => x.Employee)
                            .ThenInclude(x => x.Supervisor)
                        .Include(x => x.LeaveType)
                        .Where(x => x.EmployeeId == id || x.Employee.SupervisorId == id)
                        .ToListAsync();
                        break;
                }
                var leaveListItemViewModels = _mapper.Map<List<LeaveListItemViewModel>>(leaves);
                return DataTableResultHandler<LeaveListItemViewModel>.ResultToSsr(leaveListItemViewModels, dataTableConfiguration, DataTableConfigurationOptions.All);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<LeaveViewModel> GetLeaveById(int id)
        {
            try
            {
                var leave = await _appDbContext.Leaves
                    .Include(x => x.LeaveType)
                    .Include(x => x.Employee)
                    .Include(x => x.Reviewer)
                    .Include(x => x.DateWiseLeaves)
                    .FirstOrDefaultAsync(x => x.Id == id);
                if (leave is not null)
                {
                    return _mapper.Map<LeaveViewModel>(leave);
                }
                throw new Exception("Leave not found!");
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<DataTableResult<LeaveListItemViewModel>> GetAllLeavesSsr(DataTableConfiguration dataTableConfiguration)
        {
            try
            {
                var leaves = await _appDbContext.Leaves
                        .Include(x => x.Employee)
                            .ThenInclude(x => x.Supervisor)
                        .Include(x => x.LeaveType)
                        .ToListAsync();
                var leaveViewModels = _mapper.Map<List<LeaveListItemViewModel>>(leaves);
                return DataTableResultHandler<LeaveListItemViewModel>.ResultToSsr(leaveViewModels, dataTableConfiguration, DataTableConfigurationOptions.All);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<List<LeaveViewModel>> GetLeavesBetween(DateTime startDate, DateTime endDate)
        {
            try
            {
                var leaves = await _appDbContext.Leaves
                    .Where(x => x.FromDate >= startDate && x.FromDate <= endDate)
                    .ToListAsync();
                return _mapper.Map<List<LeaveViewModel>>(leaves);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<LeaveViewModel> DeleteLeaveById(int id)
        {
            try
            {
                var leave = await _appDbContext.Leaves
                    .FirstOrDefaultAsync(x => x.Id == id);
                if (leave is not null)
                {
                    leave.Status = DataRecordStatus.Deleted;
                    leave.DeletedBy = _accountService.GetCurrentLoggedInUserId();
                    leave.DeletedDate = DateTime.Now;
                    _appDbContext.Leaves.Update(leave);
                    await _appDbContext.SaveChangesAsync();
                    return _mapper.Map<LeaveViewModel>(leave);
                }
                throw new Exception("Leave not found!");
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<LeaveReportViewModel>> GenerateLeaveReport()
        {
            var oneYearAgo = DateTime.Today.AddYears(-1);

            var query = from lr in _appDbContext.Leaves
                        join e in _appDbContext.SystemUsers on lr.EmployeeId equals e.Id
                        join lt in _appDbContext.LeaveTypes on lr.LeaveTypeId equals lt.Id
                        where lr.FromDate >= oneYearAgo || lr.ToDate >= oneYearAgo
                        group lr by new { lt.Name, lr.LeaveTypeId, lr.FromDate.Month, lr.LeaveStatus } into g
                        orderby g.Key.Month
                        select new LeaveReportViewModel
                        {
                            Count = g.Count(),
                            LeaveTypeId = g.Key.LeaveTypeId,
                            LeaveTypeName = g.Key.Name,
                            Month = g.Key.Month,
                            LeaveStatus = g.Key.LeaveStatus,
                            LeaveStatusName = g.Key.LeaveStatus.GetDescription()
                        };

            return await query.ToListAsync();
        }
    }
}
