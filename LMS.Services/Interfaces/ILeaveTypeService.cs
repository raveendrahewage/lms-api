using LMS.Services.Common;
using LMS.Services.Responses;
using LMS.Services.ViewModels;

namespace LMS.Services.Interfaces
{
    public interface ILeaveTypeService
    {
        Task<LeaveTypeViewModel> CreateLeaveType(LeaveTypeViewModel model);
        Task<LeaveTypeViewModel> DeleteLeaveTypeById(int id);
        Task<List<LeaveTypeViewModel>> GetAllLeaveTypes();
        Task<DataTableResult<LeaveTypeViewModel>> GetAllLeaveTypesSsr(DataTableConfiguration dataTableConfiguration);
        Task<List<LeaveTypeViewModel>> GetLeavesTypeByName(string typeName);
        Task<LeaveTypeViewModel> GetLeaveTypeById(int id);
        Task<LeaveTypeViewModel> UpdateLeaveType(LeaveTypeViewModel model);
    }
}