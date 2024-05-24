using LMS.Api.ViewModels;
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
        public Task<ApiResponse<AuthResult>> SignIn(SignInViewModel model);
        public Task<ApiResponse<AuthResult>> SignUp(SignUpViewModel model);
        public Task<ApiResponse<bool>> ResetPassword(ResetPasswordViewModel model);
        public Task<ApiResponse<string>> GetPasswordResetToken();
        public Task<ApiResponse<SystemUserViewModel>> GetLoggedInSystemUser();
        public string GetCurrentLoggedInUsername();
        public int GetCurrentLoggedInUserId();
    }
}
