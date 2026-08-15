using LMS.Data.Enum;
using LMS.Data.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace LMS.Services.ViewModels
{
    public class NotificationViewModel: DataRecordViewModel
    {
        public int UserId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public NotificationType Type { get; set; }
        public string? TargetUrl { get; set; }
        public bool IsRead { get; set; } = false;
        public virtual SystemUserViewModel User { get; set; }
    }
}
