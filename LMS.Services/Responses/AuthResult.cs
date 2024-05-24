using LMS.Data.Models;
using LMS.Services.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Services.Responses
{
    public class AuthResult
    {
        public AuthResult(string? token, DateTime? expiresIn, SystemUserViewModel? user)
        {
            this.Token = token;
            this.ExpiresOn = expiresIn;
            this.User = user;
        }
        public string? Token { get; set; } = string.Empty;
        public DateTime? ExpiresOn { get; set; }
        public SystemUserViewModel? User { get; set; }
    }
}
