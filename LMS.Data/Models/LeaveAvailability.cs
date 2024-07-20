using LMS.Data.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Data.Models
{
    public class LeaveAvailability : DataRecord
    {
        public int Year { get; set; } = DateTime.UtcNow.Year;
        [ForeignKey(nameof(SystemUser))]
        public int SystemUserId { get; set; }
        [ForeignKey(nameof(LeaveType))]
        public int LeaveTypeId { get; set; }
        public int LeaveCount { get; set; }
        public int BookedCount { get; set; } = 0;
        public int BalanceCount { get; set; } = 0;

        public virtual LeaveType LeaveType { get; set; }
        public virtual SystemUser SystemUser { get; set; }
    }
}
