using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Data.Enum
{
    public enum LeaveQuarterDayType
    {
        [Description("First Quarter")]
        FirstQuarter = 1,
        [Description("Second Quarter")]
        SecondQuarter = 2,
        [Description("Third Quarter")]
        ThirdQuarter = 3,
        [Description("Fourth Quarter")]
        FourthQuarter = 4,
    }
}
