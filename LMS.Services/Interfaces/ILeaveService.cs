using LMS.Services.Common;
using LMS.Services.Constants;
using LMS.Services.Responses;
using LMS.Services.ViewModels;

namespace LMS.Services.Interfaces
{
    public interface ILeaveService
    {
        Task<LeaveViewModel> CreateLeave(LeaveViewModel model);
        Task<List<LeaveViewModel>> GetAllLeavesByEmployeeId(int id);
        Task<DataTableResult<LeaveListItemViewModel>> GetAllLeavesByEmployeeIdSsr(int id, LeaveFetchingMode leaveFetchingMode, DataTableConfiguration dataTableConfiguration);
        Task<LeaveViewModel> GetLeaveById(int id);
        Task<LeaveViewModel> UpdateLeave(LeaveViewModel model);
        Task<List<LeaveViewModel>> GetLeavesBetween(DateTime startDate, DateTime endDate);
        Task<LeaveViewModel> DeleteLeaveById(int id);
        Task<DataTableResult<LeaveViewModel>> GetAllLeaves(DataTableConfiguration dataTableConfiguration);
    }
}