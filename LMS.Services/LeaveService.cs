using Azure;
using LMS.Data;
using LMS.Data.Enum;
using LMS.Data.Extensions;
using LMS.Data.Models;
using LMS.Services.Common;
using LMS.Services.Constants;
using LMS.Services.Helpers;
using LMS.Services.Interfaces;
using LMS.Services.Responses;
using LMS.Services.ViewModels;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;

namespace LMS.Services
{
    public class LeaveService(
        ApplicationDbContext appDbContext,
        IAccountService accountService,
        INotificationService _notificationService,
        IMapper mapper
        ) : ILeaveService
    {
        private readonly ApplicationDbContext _appDbContext = appDbContext;
        private readonly IAccountService _accountService = accountService;
        private readonly INotificationService _notificationService = _notificationService;
        private readonly IMapper _mapper = mapper;

        public async Task<LeaveViewModel> CreateLeave(LeaveViewModel model)
        {
            using var transaction = await _appDbContext.Database.BeginTransactionAsync();
            try
            {
                var currentLoggedInUserId = _accountService.GetCurrentLoggedInUserId();
                var currentLoggedInUser = await _appDbContext.SystemUsers
                    .Include(x => x.Supervisor)
                    .FirstOrDefaultAsync(x => x.Id == currentLoggedInUserId) ?? throw new Exception("Logged in user not found!");

                var isConflictingLeavesAvailable = _appDbContext.Leaves.Any(x =>
                    x.EmployeeId == currentLoggedInUserId
                    && (
                        (x.FromDate <= model.FromDate && x.ToDate >= model.FromDate)
                        || (x.FromDate <= model.ToDate && x.ToDate >= model.ToDate)
                       )
                    && (x.LeaveStatus == LeaveStatus.Approved || x.LeaveStatus == LeaveStatus.Pending)
                    );
                if(isConflictingLeavesAvailable)
                    throw new Exception("Selected dates are conflicting with another leave.");

                var leaveToBeCreated = _mapper.Map<LeaveViewModel, Leave>(model);
                leaveToBeCreated.CreatedBy = currentLoggedInUserId;
                leaveToBeCreated.DateWiseLeaves.ForEach(l => l.CreatedBy = currentLoggedInUserId);
                var result = await _appDbContext.Leaves.AddAsync(leaveToBeCreated);
                await UpdateLeaveAvailabilities(leaveToBeCreated);
                await _appDbContext.SaveChangesAsync();
                await transaction.CommitAsync();
                await SendNotificationAsync(currentLoggedInUserId, currentLoggedInUser.SupervisorId, result.Entity.LeaveStatus, currentLoggedInUser.FirstName, currentLoggedInUser?.Supervisor?.FirstName, result.Entity.LeaveTypeId, result.Entity.Id);
                return _mapper.Map<Leave, LeaveViewModel>(result.Entity);
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
        public async Task<LeaveViewModel> UpdateLeave(LeaveViewModel model)
        {
            using var transaction = await _appDbContext.Database.BeginTransactionAsync();
            try
            {
                var leaveToBeUpdated = await _appDbContext.Leaves
                    .Include(x => x.Employee)
                        .ThenInclude(x => x.Supervisor)
                    .Include(x => x.DateWiseLeaves)
                    .FirstOrDefaultAsync(x => x.Id == model.Id) ?? throw new Exception("Leave not found!");

                var currentLoggedInUserId = _accountService.GetCurrentLoggedInUserId();
                var currentLoggedInUser = await _appDbContext.SystemUsers
                    .Include(x => x.Supervisor)
                    .FirstOrDefaultAsync(x => x.Id == currentLoggedInUserId) ?? throw new Exception("Logged in user not found!");
                if (leaveToBeUpdated.EmployeeId != currentLoggedInUserId && leaveToBeUpdated.Employee.SupervisorId != currentLoggedInUserId)
                    throw new Exception("You don't have permission to make changes.");

                if (leaveToBeUpdated.EmployeeId == currentLoggedInUserId)
                {
                    switch (model.LeaveStatus)
                    {
                        case LeaveStatus.Pending:
                            leaveToBeUpdated.LeaveTypeId = model.LeaveTypeId;
                            leaveToBeUpdated.Reason = model.Reason;
                            leaveToBeUpdated.FromDate = model.FromDate;
                            leaveToBeUpdated.ToDate = model.ToDate;
                            leaveToBeUpdated.LeaveStatus = LeaveStatus.Pending;
                            foreach (var dbDateWiseLeave in leaveToBeUpdated.DateWiseLeaves)
                            {
                                var modelDateWiseLeave = model.DateWiseLeaves.First(x => x.Id == dbDateWiseLeave.Id);
                                dbDateWiseLeave.LeaveDayType = modelDateWiseLeave.LeaveDayType;
                                dbDateWiseLeave.LeaveHalfDayType = modelDateWiseLeave.LeaveHalfDayType;
                                dbDateWiseLeave.LeaveQuarterDayType = modelDateWiseLeave.LeaveQuarterDayType;
                                dbDateWiseLeave.ModifiedBy = currentLoggedInUserId;
                                dbDateWiseLeave.ModifiedDate = DateTime.UtcNow;
                            }
                            await UpdateLeaveAvailabilities(leaveToBeUpdated);
                            break;
                        case LeaveStatus.Canceled:
                            leaveToBeUpdated.LeaveStatus = LeaveStatus.Canceled;
                            await UpdateLeaveAvailabilities(leaveToBeUpdated, -1);
                            break;
                    }
                    leaveToBeUpdated.ModifiedBy = currentLoggedInUserId;
                    leaveToBeUpdated.ModifiedDate = DateTime.UtcNow;
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
                            await UpdateLeaveAvailabilities(leaveToBeUpdated, -1);
                            break;
                    }
                    leaveToBeUpdated.ModifiedBy = currentLoggedInUserId;
                    leaveToBeUpdated.ModifiedDate = DateTime.UtcNow;
                }

                var result = _appDbContext.Leaves.Update(leaveToBeUpdated);
                await _appDbContext.SaveChangesAsync();
                await transaction.CommitAsync();
                await SendNotificationAsync(leaveToBeUpdated.EmployeeId, leaveToBeUpdated.Employee.SupervisorId, result.Entity.LeaveStatus, leaveToBeUpdated.Employee.FirstName, leaveToBeUpdated.Employee?.Supervisor?.FirstName, result.Entity.LeaveTypeId, leaveToBeUpdated.Id);
                return _mapper.Map<Leave, LeaveViewModel>(result.Entity);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                var entry = ex.Entries.Single();
                var clientValues = (Leave)entry.Entity;
                var databaseEntry = entry.GetDatabaseValues();

                var databaseValues = (Leave)databaseEntry.ToObject();
                await transaction.RollbackAsync();
                throw new Exception($"Leave is {Enum.GetName(typeof(LeaveStatus), databaseValues.LeaveStatus)}.");
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
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
                return _mapper.Map<List<Leave>, List<LeaveViewModel>>(leaves);
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
                leaves = leaveFetchingMode switch
                {
                    LeaveFetchingMode.All => await _appDbContext.Leaves
                                            .Include(x => x.Employee)
                                                .ThenInclude(x => x.Supervisor)
                                            .Include(x => x.LeaveType)
                                            .Where(x => x.EmployeeId == id || x.Employee.SupervisorId == id)
                                            .ToListAsync(),
                    LeaveFetchingMode.OnlyMine => await _appDbContext.Leaves
                                            .Include(x => x.Employee)
                                                .ThenInclude(x => x.Supervisor)
                                            .Include(x => x.LeaveType)
                                            .Where(x => x.EmployeeId == id)
                                            .ToListAsync(),
                    LeaveFetchingMode.OnlyApprovals => await _appDbContext.Leaves
                                            .Include(x => x.Employee)
                                                .ThenInclude(x => x.Supervisor)
                                            .Include(x => x.LeaveType)
                                            .Where(x => x.Employee.SupervisorId == id)
                                            .ToListAsync(),
                    _ => await _appDbContext.Leaves
                                            .Include(x => x.Employee)
                                                .ThenInclude(x => x.Supervisor)
                                            .Include(x => x.LeaveType)
                                            .Where(x => x.EmployeeId == id || x.Employee.SupervisorId == id)
                                            .ToListAsync(),
                };
                var leaveListItemViewModels = _mapper.Map<List<Leave>, List<LeaveListItemViewModel>>(leaves);
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
                return leave is null
                    ? throw new Exception("Leave not found!")
                    : _mapper.Map<Leave, LeaveViewModel>(leave);
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
                var leaveViewModels = _mapper.Map<List<Leave>, List<LeaveListItemViewModel>>(leaves);
                return DataTableResultHandler<LeaveListItemViewModel>.ResultToSsr(leaveViewModels, dataTableConfiguration, DataTableConfigurationOptions.All);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<List<LeaveViewModel>> GetLeavesBetween(DateOnly startDate, DateOnly endDate)
        {
            try
            {
                var leaves = await _appDbContext.Leaves
                    .Where(x => x.FromDate >= startDate && x.FromDate <= endDate)
                    .ToListAsync();
                return _mapper.Map<List<Leave>, List<LeaveViewModel>>(leaves);
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
                    .FirstOrDefaultAsync(x => x.Id == id) ?? throw new Exception("Leave not found!");

                leave.Status = DataRecordStatus.Deleted;
                leave.DeletedBy = _accountService.GetCurrentLoggedInUserId();
                leave.DeletedDate = DateTime.UtcNow;
                _appDbContext.Leaves.Update(leave);
                await _appDbContext.SaveChangesAsync();
                return _mapper.Map<Leave, LeaveViewModel>(leave);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<LeaveReportViewModel>> GenerateLeaveReport()
        {
            var oneYearAgoDateTime = DateTime.Today.AddYears(-1);
            var oneYearAgoDateOnly = new DateOnly(oneYearAgoDateTime.Year, oneYearAgoDateTime.Month, oneYearAgoDateTime.Day);

            var query = from lr in _appDbContext.Leaves
                        join e in _appDbContext.SystemUsers on lr.EmployeeId equals e.Id
                        join lt in _appDbContext.LeaveTypes on lr.LeaveTypeId equals lt.Id
                        where lr.FromDate >= oneYearAgoDateOnly || lr.ToDate >= oneYearAgoDateOnly
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

        private async Task UpdateLeaveAvailabilities(Leave leave, int multiplier = 1)
        {
            var leaveAvailability = await _appDbContext.LeaveAvailabilities
                .FirstAsync(x => x.Year == leave.FromDate.Year
                    && x.SystemUserId == leave.EmployeeId
                    && x.LeaveTypeId == leave.LeaveTypeId);
            leaveAvailability.BalanceCount -= (multiplier * leave.LeaveCount);
            leaveAvailability.BookedCount += (multiplier * leave.LeaveCount);
            _appDbContext.LeaveAvailabilities.Update(leaveAvailability);
        }

        private async Task SendNotificationAsync(int employeeId, int? supervisorId, LeaveStatus leaveStatus, string employeeName, string? supervisorName, int leaveType, int leaveId)
        {
            NotificationViewModel? notification = null;
            string targetUrl = $"/dashboard/leaves/details/{leaveType}/{leaveId}";
            if (supervisorId is not null)
            {
                if(leaveStatus == LeaveStatus.Pending)
                {
                    notification = new()
                    {
                        UserId = supervisorId.Value,
                        Title = "New Leave Application",
                        Message = $"{employeeName} submitted a new leave request.",
                        Type = NotificationType.LeaveCreated,
                        TargetUrl = targetUrl
                    };
                }

                if (leaveStatus == LeaveStatus.Canceled)
                {
                    notification = new()
                    {
                        UserId = supervisorId.Value,
                        Title = "Leave Canceled",
                        Message = $"{employeeName} canceled their leave request.",
                        Type = NotificationType.LeaveCancelled,
                        TargetUrl = targetUrl
                    };
                }
            }
            if (leaveStatus == LeaveStatus.Approved || leaveStatus == LeaveStatus.Denied)
            {
                notification = new ()
                {
                    UserId = employeeId,
                    Title = $"Leave {Enum.GetName(typeof(LeaveStatus), leaveStatus)}",
                    Message = $"{supervisorName} {Enum.GetName(typeof(LeaveStatus), leaveStatus)} your leave request.",
                    Type = leaveStatus == LeaveStatus.Approved ? NotificationType.LeaveApproved : NotificationType.LeaveDenied,
                    TargetUrl = targetUrl
                };
            }

            if(notification is not null)
                await _notificationService.SendNotificationAsync(notification);
        }
    }
}
