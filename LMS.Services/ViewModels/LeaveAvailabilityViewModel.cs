using LMS.Data.Enum;
using LMS.Data.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Services.ViewModels
{
    public class LeaveAvailabilityViewModel
    {
        public int Year { get; set; }
        public int SystemUserId { get; set; }
        public int LeaveTypeId { get; set; }
        public int LeaveCount { get; set; }
        public int BookedCount { get; set; }
        public int BalanceCount { get; set; }

        public virtual LeaveTypeViewModel LeaveType { get; set; }
        public virtual SystemUserViewModel SystemUser { get; set; }
    }
}
