using LMS.Data.Common;
using LMS.Data.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Services.ViewModels
{
    public class LeaveTypeViewModel : DataRecordViewModel
    {
        public string Name { get; set; } = string.Empty;
        public int DefaultLeaveCount { get; set; } = 0;
        public virtual List<LeaveViewModel> Leaves { get; } = [];
        public virtual List<LeaveAvailabilityViewModel> LeaveAvailabilities { get; } = [];
    }
}
