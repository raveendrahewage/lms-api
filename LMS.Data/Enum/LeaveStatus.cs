using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Data.Enum
{
    public enum LeaveStatus
    {
        [Description("Pending")]
        Pending = 1,
        [Description("Approved")]
        Approved = 2,
        [Description("Denied")]
        Denied = 3,
        [Description("Canceled")]
        Canceled = 4
    }
}
