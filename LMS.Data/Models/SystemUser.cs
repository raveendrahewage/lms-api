using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LMS.Data.Common;
using LMS.Data.Enum;
using Microsoft.AspNetCore.Identity;

namespace LMS.Data.Models
{
    public class SystemUser : IdentityUser<int>
    {
        [Required]
        [Column(TypeName = "nvarchar(150)")]
        public string FirstName { get; set; } = string.Empty;
        [Required]
        [Column(TypeName = "nvarchar(150)")]
        public string LastName { get; set; } = string.Empty;
        [Required]
        [Column(TypeName = "nvarchar(200)")]
        public string Email { get; set; } = string.Empty;
        [Column(TypeName = "nvarchar(20)")]
        public string? PhoneNumber { get; set; }
        [ForeignKey(nameof(Role))]
        public int RoleId {  get; set; }
        [ForeignKey(nameof(Supervisor))]
        public int? SupervisorId { get; set; }
        [Required]
        public int CreatedBy { get; set; }
        [Required]
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public int? DeletedBy { get; set; }
        public DateTime? DeletedDate { get; set; }
        public DataRecordStatus Status { get; set; } = DataRecordStatus.Active;
        public virtual SystemUser? Supervisor { get; set; }
        public virtual List<SystemUser> EmployeesUnderSupervision { get; set; } = new List<SystemUser>();
        public virtual SystemRole Role { get; set; }
    }
}
