using LMS.Data.Common;
using LMS.Data.Enum;
using LMS.Data.Models;
using LMS.Services.Helpers;
using Newtonsoft.Json;
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
        public int LeaveTypeId { get; set; }
        [JsonConverter(typeof(DateOnlyJsonConverter))]
        public DateOnly FromDate { get; set; }
        [JsonConverter(typeof(DateOnlyJsonConverter))]
        public DateOnly ToDate { get; set; }
        public string Reason { get; set; } = string.Empty;
        public LeaveStatus LeaveStatus { get; set; } = LeaveStatus.Pending;
        public bool IsApproved { get; set; }
        public string? DeniedReason { get; set; }
        public int? ReviewedBy { get; set; }

        public virtual List<DateWiseLeaveViewModel> DateWiseLeaves { get; } = [];
        public virtual LeaveTypeViewModel? LeaveType { get; set; }
        public virtual SystemUserViewModel? Reviewer { get; set; }
        public virtual SystemUserViewModel? Employee { get; set; }

        public double LeaveCount
        {
            get
            {
                return DateWiseLeaves.Sum(d => d.LeaveCount);
            }
        }
    }
}
