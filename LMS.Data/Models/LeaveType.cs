using LMS.Data.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Data.Models
{
    public class LeaveType : DataRecord
    {
        [Column(TypeName = "nvarchar(150)")]
        public string Name { get; set; } = string.Empty;
        public int DefaultLeaveCount { get; set; } = 0;
        public virtual List<Leave> Leaves { get; set; } = [];
        public virtual List<LeaveAvailability> LeaveAvailabilities { get; set; } = [];
    }
}
