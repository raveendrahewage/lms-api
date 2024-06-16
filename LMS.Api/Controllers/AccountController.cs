using LMS.Api.ViewModels;
using LMS.Data.Enum;
using LMS.Services.Helpers.Interfaces;
using LMS.Services.Interfaces;
using LMS.Services.Responses;
using LMS.Services.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace LMS.Api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;
        private readonly IApiResponseHelper _apiResponseHelper;
        public AccountController(IAccountService accountService, IApiResponseHelper apiResponseHelper) {
            _accountService = accountService;
            _apiResponseHelper = apiResponseHelper;
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("sign-in")]
        public async Task<IActionResult> SignIn(SignInViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var result = await _accountService.SignIn(model);
                    return Ok(_apiResponseHelper.GenerateApiResponse(true, "Signed in successfully!", result));
                }
                return BadRequest(model);
            }
            catch (Exception) {
                throw;
            }
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("sign-up")]
        public async Task<IActionResult> SignUp(SignUpViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var result = await _accountService.SignUp(model);
                    return Ok(_apiResponseHelper.GenerateApiResponse(true, "Signed up successfully!", result));
                }
                return BadRequest(model);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet]
        [Route("get-logged-in-system-user")]
        public async Task<IActionResult> GetLoggedInSystemUser()
        {
            try
            {
                var result = await _accountService.GetLoggedInSystemUser();
                return Ok(_apiResponseHelper.GenerateApiResponse(true, result));
            }
            catch (Exception)
            {
                throw;
            }
        }
        [HttpPost]
        [Route("reset-password")]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var result = await _accountService.ResetPassword(model);
                    return Ok(_apiResponseHelper.GenerateApiResponse(true, "Password reset successfully!", result));
                }
                return BadRequest(model);
            }
            catch (Exception)
            {
                throw;
            }
        }
        [HttpGet]
        [Route("get-password-reset-token")]
        public async Task<IActionResult> GetPasswordResetToken()
        {
            try
            {
                var result = await _accountService.GetPasswordResetToken();
                return Ok(_apiResponseHelper.GenerateApiResponse(true, result));
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
