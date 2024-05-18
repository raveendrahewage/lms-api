using LMS.Data.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Data.Models
{
    public class Leaves: DataRecord
    {
        public int EmployeeId { get; set; }
        [ForeignKey(nameof(LeaveType))]
        public int LeaveTypeId { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        [Column(TypeName = "nvarchar(max)")]
        public string Reason { get; set; } = string.Empty;
        public bool IsApproved { get; set; }
        [Column(TypeName = "nvarchar(max)")]
        public string? DeniedReason { get; set; }
        [ForeignKey(nameof(SystemUser))]
        public int? ReviewedBy { get; set; }

        public virtual required LeaveType LeaveType { get; set; }
        public virtual SystemUser? Supervisor { get; set; }
    }
}
