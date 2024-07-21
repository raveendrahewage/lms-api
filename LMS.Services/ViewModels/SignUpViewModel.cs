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
    public class SignUpViewModel
    {
        [Required]
        public string FirstName { get; set; } = string.Empty;
        [Required]
        public string LastName { get; set; } = string.Empty;
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        [Required]
        public string Password { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        [Required]
        public int RoleId { get; set; }
        public int? SupervisorId { get; set; }
        public virtual List<SystemUserViewModel> EmployeesUnderSupervision { get; set; } = [];
        public virtual List<LeaveAvailabilityViewModel> LeaveAvailabilities { get; set; } = [];
    }
}
