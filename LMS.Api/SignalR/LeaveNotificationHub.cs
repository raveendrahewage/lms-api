using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace LMS.Api.SignalR
{
    [Authorize]
    public class LeaveNotificationHub: Hub
    {
    }
}
