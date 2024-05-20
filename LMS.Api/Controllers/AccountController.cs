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
        // GET: api/<AccountController>
        [HttpGet]
        [Route("")]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<AccountController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<AccountController>
        [HttpPost]
        [Route("sign-in")]
        public async Task<IActionResult> SignIn([FromBody] SignInViewModel model)
        {
            if (ModelState.IsValid) { 
                var result = await _accountService.SignIn(model);
                if (result.Success) {
                    SetTokenCookieAndHeader(result.Data.Token);
                    return Ok(result);
                } else return Unauthorized(result);
            }
            return BadRequest(model);
        }

        [HttpPost]
        [Route("sign-up")]
        public async Task<IActionResult> SignUp([FromBody] SignUpViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _accountService.SignUp(model);
                return result.Success ? Ok(result) : BadRequest(result);
            }
            return BadRequest(model);
        }

        // PUT api/<AccountController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<AccountController>/5
        [HttpDelete("{id}")]
        [Authorize]
        public void Delete(int id)
        {
            var currentUserId = User.FindFirstValue(AuthClaims.SysUserUserId);
        }
        private void SetTokenCookieAndHeader(string token)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddDays(7)
            };
            Response.Headers.Append("Authorization", $"Bearer {token}");
            Response.Headers.Append("Content-Type", "application/json");
            Response.Cookies.Append("token", token, cookieOptions);
        }
    }
}
