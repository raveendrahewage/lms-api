using LMS.Services.Responses;
using LMS.Services.ViewModels;

namespace LMS.Services.Interfaces
{
    public interface IEmployeeService
    {
        Task<SystemUserViewModel> CreateNewEmployee(SignUpViewModel model);
        Task<SystemUserViewModel> GetEmployeeById(int id);
        Task<List<SystemUserViewModel>> GetAllEmployees(int? page, int? size);
        Task<SystemUserViewModel> GetEmployeeByFullName(string fullName);
        Task<List<SystemUserViewModel>> GetEmployeesUnderSupervision(int id);
        Task<SystemUserViewModel> UpdateEmployee(SystemUserViewModel model);
        public Task<SystemUserViewModel> DeleteEmployeeById(int id);
        public Task<SystemUserViewModel> InactivateEmployeeById(int id);
    }
}