using LMS.Data.Enum;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace LMS.Api.SignalR
{
    public class CustomUserIdProvider: IUserIdProvider
    {
        public string GetUserId(HubConnectionContext connection)
        {
            return connection.User?.FindFirst(AuthClaim.SysUserUserId)?.Value ?? "0";
        }
    }
}
