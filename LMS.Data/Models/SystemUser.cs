using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LMS.Data.Common;
using LMS.Data.CoreIdentity;
using Microsoft.AspNetCore.Identity;

namespace LMS.Data.Models
{
    public class SystemUser : DataRecord
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
        public string? Phone { get; set; }
        [ForeignKey(nameof(FrameworkRole))]
        public int FrameworkRoleId {  get; set; }
        [ForeignKey(nameof(Supervisor))]
        public int? SupervisorId { get; set; }
        [ForeignKey("FrameworkUser")]
        public int FrameworkUserId { get; set; }

        public virtual CoreIdentityUser FrameworkUser { get; set; }
        public virtual SystemUser? Supervisor { get; set; }
        public virtual List<SystemUser> EmployeesUnderSupervision { get; set; } = new List<SystemUser>();
        public virtual SystemRole FrameworkRole { get; set; }
    }
}
