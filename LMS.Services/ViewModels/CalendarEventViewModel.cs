using LMS.Data.Enum;
using LMS.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Services.ViewModels
{
    public class CalendarEventViewModel
    {
        public int CalendarEventId { get; set; }
        public string Title { get; set; } = string.Empty;
        public CalendarEventType CalendarEventType { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
