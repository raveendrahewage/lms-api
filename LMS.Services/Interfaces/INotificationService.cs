using LMS.Services.Common;
using LMS.Services.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.Services.Interfaces
{
    public interface INotificationService
    {
        Task SendNotificationAsync(NotificationViewModel model);
        Task<List<NotificationViewModel>> GetAllNotificationsByEmployeeId(int id, int take);
        Task<DataTableResult<NotificationViewModel>> GetAllNotificationsByEmployeeIdSsr(int id, DataTableConfiguration dataTableConfiguration);
        Task<NotificationViewModel> MarkAsRead(int id, int userId);
        Task<List<NotificationViewModel>> MarkAllAsRead(int userId);
        Task<int> GetUnreadCount(int userId);
    }
}
