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
        [Column(TypeName = "decimal(18, 2)")]
        public double LeaveCount { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public double BookedCount { get; set; } = 0;
        [Column(TypeName = "decimal(18, 2)")]
        public double BalanceCount { get; set; } = 0;

        public virtual LeaveType LeaveType { get; set; }
        public virtual SystemUser SystemUser { get; set; }
    }
}
