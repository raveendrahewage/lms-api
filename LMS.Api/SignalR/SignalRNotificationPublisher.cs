using LMS.Services.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace LMS.Api.SignalR
{
    public class SignalRNotificationPublisher : INotificationPublisher
    {
        private readonly IHubContext<NotificationHub> _hubContext;

        public SignalRNotificationPublisher(IHubContext<NotificationHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task PublishToUserAsync(string userId, string method, object payload)
        {
            await _hubContext.Clients.User(userId).SendAsync(method, payload);
        }
    }
}
