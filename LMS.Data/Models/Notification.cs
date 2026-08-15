using LMS.Data.Common;
using LMS.Data.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace LMS.Data.Models
{
    public class Notification: DataRecord
    {
        [ForeignKey(nameof(User))]
        public int UserId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public NotificationType Type { get; set; }
        public string? TargetUrl { get; set; }
        public bool IsRead { get; set; } = false;
        public virtual SystemUser User { get; set; }
    }
}
