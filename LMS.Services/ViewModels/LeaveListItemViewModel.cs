using LMS.Data.Enum;
using LMS.Services.Helpers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Services.ViewModels
{
    public class LeaveListItemViewModel: DataRecordViewModel
    {
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; } = string.Empty;
        public int LeaveTypeId { get; set; }
        public string LeaveTypeName { get; set; } = string.Empty;
        public int? SupervisorId { get; set; }
        public string? SupervisorName { get; set; } = string.Empty;
        [JsonConverter(typeof(DateOnlyJsonConverter))]
        public DateOnly FromDate { get; set; }
        [JsonConverter(typeof(DateOnlyJsonConverter))]
        public DateOnly ToDate { get; set; }
        public LeaveStatus LeaveStatus { get; set; } = LeaveStatus.Pending;
    }
}
