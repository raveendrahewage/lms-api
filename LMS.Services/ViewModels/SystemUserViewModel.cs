using LMS.Data.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Services.ViewModels
{
    public class SystemUserViewModel: DataRecordViewModel
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public int RoleId { get; set; }
        public int? SupervisorId { get; set; }
        public string FullName => $"{FirstName} {LastName}";
        public virtual SystemUserViewModel? Supervisor { get; set; }
        public virtual List<SystemUserViewModel> EmployeesUnderSupervision { get; set; } = [];
        public virtual SystemRoleViewModel? Role { get; set; }
        public virtual List<LeaveViewModel> Leaves { get; set; } = [];
        public virtual List<LeaveViewModel> ReviewedLeaves { get; set; } = [];
    }
}
