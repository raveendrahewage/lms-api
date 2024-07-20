using LMS.Data.Common;
using LMS.Data.Enum;
using LMS.Data.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Data.Models
{
    public class DateWiseLeave:DataRecord
    {
        [ForeignKey(nameof(Leave))]
        public int LeaveId { get; set; }
        public DateOnly Date { get; set; }
        public LeaveDayType LeaveDayType { get; set; } = LeaveDayType.FullDay;
        public LeaveHalfDayType? LeaveHalfDayType { get; set; }
        public LeaveQuarterDayType? LeaveQuarterDayType { get; set; }
        [Timestamp]
        public byte[] Version { get; set; } = [];

        public virtual Leave Leave { get; set; }

        [NotMapped]
        public string Title
        {
            get
            {
                var leaveDayDescription = LeaveDayType.GetDescription();
                var leaveHalfDayDescription = LeaveHalfDayType?.GetDescription() ?? string.Empty;
                var leaveQuarterDayDescription = LeaveQuarterDayType?.GetDescription() ?? string.Empty;
                var leaveType = Leave?.LeaveType?.Name ?? "Unknown Leave Type";
                var leaveDescription = LeaveDayType != LeaveDayType.FullDay ? string.IsNullOrEmpty(leaveHalfDayDescription) ? $"({leaveQuarterDayDescription})" : $"({leaveHalfDayDescription})" : string.Empty;

                return $"{leaveType} - {leaveDayDescription} {leaveDescription}".Trim();
            }
        }
    }
}
