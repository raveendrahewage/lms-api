using LMS.Api.Helpers;
using LMS.Api.Helpers.Interfaces;
using LMS.Data;
using LMS.Data.Enum;
using LMS.Services.Interfaces;
using LMS.Services.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace LMS.Api.Controllers
{
    [ApiController]
    [Route("api/notifications")]
    [Authorize]
    public class NotificationController(INotificationService notificationService, IApiResponseHelper _apiResponseHelper) : ControllerBase
    {
        private readonly INotificationService _notificationService = notificationService;
        private readonly IApiResponseHelper _apiResponseHelper = _apiResponseHelper;

        [HttpGet]
        public async Task<IActionResult> GetNotifications([FromQuery] int take = 10)
        {
            var userId = int.TryParse(User.FindFirstValue(AuthClaim.SysUserUserId), out int parsedUserId) ? parsedUserId : 0;
            var response = new NotificationsListViewModel
            {
                Notifications = await _notificationService.GetAllNotificationsByEmployeeId(userId, take),
                UnreadCount = await _notificationService.GetUnreadCount(userId)
            };

            return Ok(_apiResponseHelper.GenerateApiResponse(true, response));
        }

        [HttpPut("read/{id}")]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            var userId = int.TryParse(User.FindFirstValue(AuthClaim.SysUserUserId), out int parsedUserId) ? parsedUserId : 0;
            var notification = await _notificationService.MarkAsRead(id, userId);

            return Ok(_apiResponseHelper.GenerateApiResponse(true, notification));
        }

        [HttpPut("read-all")]
        public async Task<IActionResult> MarkAllAsRead()
        {
            var userId = int.TryParse(User.FindFirstValue(AuthClaim.SysUserUserId), out int parsedUserId) ? parsedUserId : 0;
            var notifications = await _notificationService.MarkAllAsRead(userId);

            return Ok(_apiResponseHelper.GenerateApiResponse(true, notifications));
        }
    }
}