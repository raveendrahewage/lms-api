using LMS.Services.Responses;
using LMS.Services.ViewModels;

namespace LMS.Services.Interfaces
{
    public interface ILeaveService
    {
        Task<LeaveViewModel> CreateLeave(LeaveViewModel model);
        Task<List<LeaveViewModel>> GetAllLeavesByEmployeeId(int id);
        Task<LeaveTypeViewModel> GetLeaveById(int id);
        Task<LeaveViewModel> UpdateLeave(LeaveViewModel model);
        Task<List<LeaveViewModel>> GetLeavesBetween(DateTime startDate, DateTime endDate);
        Task<LeaveViewModel> DeleteLeaveById(int id);
        Task<List<LeaveViewModel>> GetAllLeaves(int? page, int? size);
    }
}