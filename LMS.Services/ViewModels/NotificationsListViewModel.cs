using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.Services.ViewModels
{
    public class NotificationsListViewModel
    {
        public List<NotificationViewModel> Notifications { get; set; }
        public int UnreadCount { get; set; }
    }
}
