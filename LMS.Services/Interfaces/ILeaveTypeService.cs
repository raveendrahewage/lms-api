using LMS.Services.Responses;
using LMS.Services.ViewModels;

namespace LMS.Services.Interfaces
{
    public interface ILeaveTypeService
    {
        Task<ApiResponse<LeaveTypeViewModel>> CreateLeaveType(LeaveTypeViewModel model);
        Task<ApiResponse<LeaveTypeViewModel>> DeleteLeaveTypeById(int id);
        Task<ApiResponse<List<LeaveTypeViewModel>>> GetAllLeaveTypes();
        Task<ApiResponse<List<LeaveTypeViewModel>>> GetLeavesTypeByName(string typeName);
        Task<ApiResponse<LeaveTypeViewModel>> GetLeaveTypeById(int id);
        Task<ApiResponse<LeaveTypeViewModel>> UpdateLeaveType(LeaveTypeViewModel model);
    }
}