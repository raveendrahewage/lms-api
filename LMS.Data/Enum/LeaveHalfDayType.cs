using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Data.Enum
{
    public enum LeaveHalfDayType
    {
        [Description("First Half")]
        FirstHalf = 1,
        [Description("Second Half")]
        SecondHalf = 2
    }
}
