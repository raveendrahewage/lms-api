using LMS.Data.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Services.ViewModels
{
    public class LeaveReportViewModel
    {
        public int Count { get; set; }
        public int LeaveTypeId { get; set; }
        public string LeaveTypeName { get; set; } = string.Empty;
        public int Month { get; set; }
        public LeaveStatus LeaveStatus { get; set; }
        public string LeaveStatusName { get; set; } = string.Empty;
    }
}
