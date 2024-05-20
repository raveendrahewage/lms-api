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
        public Task<ApiResponse<AuthResponse>> SignIn(SignInViewModel model);
        public Task<ApiResponse<AuthResponse>> SignUp(SignUpViewModel model);
    }
}
