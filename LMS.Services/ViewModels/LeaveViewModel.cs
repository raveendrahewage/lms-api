using LMS.Data.Common;
using LMS.Data.Enum;
using LMS.Data.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Services.ViewModels
{
    public class LeaveViewModel: DataRecordViewModel
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
        [ForeignKey(nameof(Supervisor))]
        public int? ReviewedBy { get; set; }

        public virtual List<DateWiseLeaveViewModel> DateWiseLeaves { get; } = [];
        public virtual LeaveTypeViewModel LeaveType { get; set; }
        public virtual SystemUserViewModel? Supervisor { get; set; }
        public virtual SystemUserViewModel Employee { get; set; }
    }
}
