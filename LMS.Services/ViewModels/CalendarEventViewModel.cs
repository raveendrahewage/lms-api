using LMS.Data.Enum;
using LMS.Data.Models;
using LMS.Services.Helpers;
using Newtonsoft.Json;
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
        public int? SupervisorId { get; set; }
        public int SystemUserId { get; set; }
        public string Title { get; set; } = string.Empty;
        public CalendarEventType CalendarEventType { get; set; }
        [JsonConverter(typeof(DateOnlyJsonConverter))]
        public DateOnly StartDate { get; set; }
        [JsonConverter(typeof(DateOnlyJsonConverter))]
        public DateOnly EndDate { get; set; }
    }
}
