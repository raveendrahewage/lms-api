using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.Services.Interfaces
{
    public interface INotificationPublisher
    {
        Task PublishToUserAsync(string userId, string method, object payload);
    }
}
