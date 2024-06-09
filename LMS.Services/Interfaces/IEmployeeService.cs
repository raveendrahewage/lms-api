using LMS.Services.Responses;
using LMS.Services.ViewModels;

namespace LMS.Services.Interfaces
{
    public interface IEmployeeService
    {
        Task<ApiResponse<SystemUserViewModel>> CreateNewEmployee(SignUpViewModel model);
        Task<ApiResponse<SystemUserViewModel>> GetEmployeeById(int id);
        Task<ApiResponse<List<SystemUserViewModel>>> GetAllEmployees(int page, int size);
        Task<ApiResponse<SystemUserViewModel>> GetEmployeeByFullName(string fullName);
        Task<ApiResponse<List<SystemUserViewModel>>> GetEmployeesUnderSupervision(int id);
        Task<ApiResponse<SystemUserViewModel>> UpdateEmployee(SystemUserViewModel model);
        public Task<ApiResponse<SystemUserViewModel>> DeleteEmployeeById(int id);
        public Task<ApiResponse<SystemUserViewModel>> InactivateEmployeeById(int id);
    }
}