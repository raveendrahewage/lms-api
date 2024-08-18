using LMS.Data.Common;
using LMS.Data.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Data.Models
{
    public class Leave: DataRecord
    {
        [ForeignKey(nameof(Employee))]
        public int EmployeeId { get; set; }
        [ForeignKey(nameof(LeaveType))]
        public int LeaveTypeId { get; set; }
        public DateOnly FromDate { get; set; }
        public DateOnly ToDate { get; set; }
        [Column(TypeName = "nvarchar(max)")]
        public string Reason { get; set; } = string.Empty;
        public LeaveStatus LeaveStatus { get; set; } = LeaveStatus.Pending;
        [Column(TypeName = "nvarchar(max)")]
        public string? DeniedReason { get; set; }
        [ForeignKey(nameof(Reviewer))]
        public int? ReviewedBy { get; set; }
        [Timestamp]
        public byte[] Version { get; set; }

        public virtual List<DateWiseLeave> DateWiseLeaves { get; set; } = [];
        public virtual LeaveType LeaveType { get; set; }
        public virtual SystemUser? Reviewer { get; set; }
        public virtual SystemUser Employee { get; set; }

        [NotMapped]
        public decimal LeaveCount
        {
            get
            {
                return DateWiseLeaves.Sum(d => d.LeaveCount);
            }
        }
    }
}
