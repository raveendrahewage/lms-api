using LMS.Api.ViewModels;
using LMS.Data.Models;
using LMS.Services.Responses;
using LMS.Services.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Services.Interfaces
{
    public interface IAccountService
    {
        public Task<AuthResult> SignIn(SignInViewModel model);
        public Task<AuthResult> SignUp(SignUpViewModel model);
        public Task<bool> ResetPassword(ResetPasswordViewModel model);
        public Task<string> GetPasswordResetToken();
        public Task<SystemUserViewModel> GetLoggedInSystemUser();
        public string GetCurrentLoggedInUsername();
        public int GetCurrentLoggedInUserId();
        public SystemUser? GetCurrentLoggedInUser();
    }
}
