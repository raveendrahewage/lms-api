using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Data.Enum
{
    public enum LeaveDayType
    {
        [Description("Full Day")]
        FullDay = 1,
        [Description("Half Day")]
        HalfDay = 2,
        [Description("Quarter Day")]
        QuarterDay = 3
    }
}
