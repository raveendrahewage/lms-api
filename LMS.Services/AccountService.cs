using LMS.Data.CoreIdentity;
using LMS.Data.Enum;
using LMS.Data.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Services
{
    public class AccountService
    {
        private readonly DbContext _context;
        private readonly RoleManager<CoreIdentityRole> _roleManager;
        private readonly SignInManager<CoreIdentityUser> _signInManager;
        private readonly UserManager<CoreIdentityUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AccountService(
            DbContext context,
            RoleManager<CoreIdentityRole> roleManager,
            SignInManager<CoreIdentityUser> signInManager,
            UserManager<CoreIdentityUser> userManager,
            IHttpContextAccessor httpContextAccessor
        )
        {
            _context = context;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<bool> SignInAsync(string username, string password, bool remember,
            bool lockoutOnFailure = false)
        {
            var signInResult = await _signInManager.PasswordSignInAsync(username, password, remember, lockoutOnFailure);
            if (signInResult != null && signInResult.Succeeded) return true;

            return false;
        }
        public string GetCurrentLoggedInUsername()
        {
            return _httpContextAccessor.HttpContext.User.Claims.First(x => x.Type == AuthClaims.SysUserUsername)
                .Value;
        }
        public int GetCurrentLoggedInUserId()
        {
            return Convert.ToInt32(_httpContextAccessor.HttpContext.User.Claims.First(x => x.Type == AuthClaims.SysUserUserId).Value);
        }
    }
}
