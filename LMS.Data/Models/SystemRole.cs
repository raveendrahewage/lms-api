using LMS.Data.Common;
using LMS.Data.CoreIdentity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Data.Models
{
    public class SystemRole: DataRecord
    {
        [Column(TypeName = "nvarchar(150)")]
        public string Name { get; set; } = string.Empty;
        [ForeignKey("FrameworkRole")]
        public int FrameworkRoleId { get; set; }
        public virtual CoreIdentityRole? FrameworkRole { get; set; }
    }
}
