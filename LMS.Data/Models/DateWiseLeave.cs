using LMS.Data.Common;
using LMS.Data.Enum;
using System;
using System.Collections.Generic;
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
        public DateTime Date { get; set; }
        public LeaveDayType LeaveDayType { get; set; } = LeaveDayType.FullDay;
        public LeaveHalfDayType? LeaveHalfDayType { get; set; }
        public LeaveQuarterDayType? LeaveQuarterDayType { get; set; }

        public virtual Leave Leave { get; set; }
    }
}
