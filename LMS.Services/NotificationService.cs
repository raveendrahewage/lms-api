using LMS.Data;
using LMS.Data.Models;
using LMS.Services.Common;
using LMS.Services.Constants;
using LMS.Services.Helpers;
using LMS.Services.Interfaces;
using LMS.Services.ViewModels;
using MapsterMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using System.Text;

namespace LMS.Services
{
    public class NotificationService(
        ApplicationDbContext appDbContext,
        INotificationPublisher _notificationPublisher,
        IMapper mapper
        ) :INotificationService
    {
        public readonly ApplicationDbContext _appDbContext = appDbContext;
        private readonly INotificationPublisher _notificationPublisher = _notificationPublisher;
        private readonly IMapper _mapper = mapper;

        public async Task SendNotificationAsync(NotificationViewModel model)
        {
            var newNotification = _mapper.Map<NotificationViewModel, Notification>(model);
            var result = _appDbContext.Notifications.Add(newNotification);
            await _appDbContext.SaveChangesAsync();

            var payload = new
            {
                id = result.Entity.Id,
                title = result.Entity.Title,
                message = result.Entity .Message,
                type = result.Entity.Type,
                targetUrl = result.Entity.TargetUrl,
                isRead = result.Entity.IsRead,
                createdAt = result.Entity.CreatedDate
            };
            await _notificationPublisher.PublishToUserAsync(result.Entity.UserId.ToString(), "ReceiveNotification", payload);
        }

        public async Task<List<NotificationViewModel>> GetAllNotificationsByEmployeeId(int id, int take)
        {
            try
            {
                var notifications = await _appDbContext.Notifications
                    .Where(x => x.UserId == id)
                    .OrderByDescending(x => x.CreatedDate)
                    .Take(take)
                    .ToListAsync();
                return _mapper.Map<List<Notification>, List<NotificationViewModel>>(notifications);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<DataTableResult<NotificationViewModel>> GetAllNotificationsByEmployeeIdSsr(int id, DataTableConfiguration dataTableConfiguration)
        {
            try
            {
                var notifications = await _appDbContext.Notifications
                    .Where(x => x.UserId == id)
                    .OrderByDescending(x => x.CreatedDate)
                    .ToListAsync();
                var notificationViewModels = _mapper.Map<List<Notification>, List<NotificationViewModel>>(notifications);
                return DataTableResultHandler<NotificationViewModel>.ResultToSsr(notificationViewModels, dataTableConfiguration, DataTableConfigurationOptions.All);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<NotificationViewModel> MarkAsRead(int id, int userId)
        {
            var notification = await _appDbContext.Notifications
                .FirstOrDefaultAsync(n => n.Id == id && n.UserId == userId);

            if (notification is not null)
            {
                notification.IsRead = true;
                await _appDbContext.SaveChangesAsync();
            }

            return _mapper.Map<Notification, NotificationViewModel>(notification);
        }

        public async Task<List<NotificationViewModel>> MarkAllAsRead(int userId)
        {
            var unreadNotifications = await _appDbContext.Notifications
                .Where(n => n.UserId == userId && !n.IsRead)
                .ToListAsync();

            foreach (var item in unreadNotifications)
                item.IsRead = true;

            await _appDbContext.SaveChangesAsync();
            return _mapper.Map<List<Notification>, List<NotificationViewModel>>(unreadNotifications);
        }

        public async Task<int> GetUnreadCount(int userId)
        {
            var unreadNotificationsCount = await _appDbContext.Notifications
                .CountAsync(n => n.UserId == userId && !n.IsRead);
            return unreadNotificationsCount;
        }
    }
}
