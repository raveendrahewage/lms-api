using LMS.Api.ViewModels;
using LMS.Data.Enum;
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
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;
        public AccountController(IAccountService accountService) {
            _accountService = accountService;
        }

        [HttpPost]
        [Route("sign-in")]
        public async Task<IActionResult> SignIn(SignInViewModel model)
        {
            if (ModelState.IsValid) { 
                var result = await _accountService.SignIn(model);
                return result.Success ? Ok(result) : Unauthorized(result);
            }
            return BadRequest(model);
        }

        [HttpPost]
        [Route("sign-up")]
        public async Task<IActionResult> SignUp(SignUpViewModel model)
        {
            if (ModelState.IsValid)
            {
                return Ok(await _accountService.SignUp(model));
            }
            return BadRequest(model);
        }

        [HttpGet]
        [Route("get-logged-in-system-user")]
        public async Task<IActionResult> GetLoggedInSystemUser()
        {
            return Ok(await _accountService.GetLoggedInSystemUser());
        }
        [HttpPost]
        [Route("reset-password")]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                return Ok(await _accountService.ResetPassword(model));
            }
            return BadRequest(model);
        }
        [HttpGet]
        [Route("get-password-reset-token")]
        public async Task<IActionResult> GetPasswordResetToken()
        {
            return Ok(await _accountService.GetPasswordResetToken());
        }
    }
}
