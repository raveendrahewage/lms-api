using AutoMapper;
using Azure;
using LMS.Api.ViewModels;
using LMS.Data.Enum;
using LMS.Data.Models;
using LMS.Services.Interfaces;
using LMS.Services.Responses;
using LMS.Services.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Services
{
    public class AccountService: IAccountService
    {
        private readonly DbContext _context;
        private readonly RoleManager<SystemRole> _roleManager;
        private readonly SignInManager<SystemUser> _signInManager;
        private readonly UserManager<SystemUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AccountService(
            DbContext context,
            RoleManager<SystemRole> roleManager,
            SignInManager<SystemUser> signInManager,
            UserManager<SystemUser> userManager,
            IConfiguration configuration,
            IMapper mapper,
            IHttpContextAccessor httpContextAccessor
        )
        {
            _context = context;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _userManager = userManager;
            _configuration = configuration;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<ApiResponse<AuthResponse>> SignIn(SignInViewModel model)
        {
            var signInResult = await SignInAsync(model, false);
            if (signInResult)
            {
                var systemUser = await _userManager.FindByEmailAsync(model.Email);
                var systemUserViewModel = _mapper.Map<SystemUser, SystemUserViewModel>(systemUser);
                var token = GenerateToken(systemUser);
                return new ApiResponse<AuthResponse>(
                    signInResult,
                    "Signed in successfully!",
                    new AuthResponse(
                        new JwtSecurityTokenHandler().WriteToken(token),
                        token.ValidTo,
                        systemUserViewModel
                    )
                );
            }
            return new ApiResponse<AuthResponse>(signInResult, "Signed in failed! Incorrect username or password.");
        }

        public async Task<ApiResponse<AuthResponse>> SignUp(SignUpViewModel model)
        {
            var signUpResult = await SignUpAsync(model);
            if (signUpResult)
            {
                var systemUser = await _userManager.FindByEmailAsync(model.Email);
                var systemUserViewModel = _mapper.Map<SystemUser, SystemUserViewModel>(systemUser);
                var token = GenerateToken(systemUser);
                return new ApiResponse<AuthResponse>(
                    signUpResult,
                    "Signed up successfully!",
                    new AuthResponse (
                        new JwtSecurityTokenHandler().WriteToken(token),
                        token.ValidTo,
                        systemUserViewModel
                    )
                );
            }
            return new ApiResponse<AuthResponse>(signUpResult, "Signed up failed! Something went wrong.");
        }
        public async Task<bool> SignInAsync(SignInViewModel model, bool lockoutOnFailure = false)
        {
            var signInResult = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure);
            if (signInResult != null && signInResult.Succeeded) return true;

            return false;
        }

        public async Task<bool> SignUpAsync(SignUpViewModel model)
        {
            var newSystemUser = _mapper.Map<SignUpViewModel, SystemUser>(model);
            newSystemUser.SupervisorId = model.SupervisorId is not null && model.SupervisorId > 0 ? model.SupervisorId : null;
            var signUpResult = await _userManager.CreateAsync(newSystemUser, model.Password);
            if (signUpResult != null && signUpResult.Succeeded) return true;
            return false;
        }
        //private JwtSecurityToken GenerateJwtSecurityToken(SystemUser systemUser)
        //{
        //    var claims = new List<Claim>();

        //    claims.Add(new Claim(AuthClaims.SysUserUsername, systemUser.Email));
        //    claims.Add(new Claim(AuthClaims.SysUserUserId, systemUser.Id.ToString()));
        //    claims.Add(new Claim(AuthClaims.SysUserRole, systemUser.Role.FrameworkRole.Name));
        //    claims.Add(new Claim(AuthClaims.SysUserRoleId, systemUser.Role.FrameworkRole.Id.ToString()));

        //    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtConfigurations.Key));
        //    var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        //    var token = new JwtSecurityToken(
        //        _jwtConfigurations.Issuer,
        //        _jwtConfigurations.Audience,
        //        claims,
        //        expires: DateTime.UtcNow.AddMinutes(_jwtConfigurations.Expires),
        //        signingCredentials: credentials);
        //    return token;
        //}
        private SecurityToken GenerateToken(SystemUser systemUser)
        {
            var key = Encoding.UTF8.GetBytes(_configuration.GetSection("JWTSetting").GetSection("SecurityKey").Value!);
            List<Claim> claims =
            [
                new Claim(AuthClaims.SysUserUsername, systemUser.Email),
                new Claim(AuthClaims.SysUserUserId, systemUser.Id.ToString()),
                new Claim(AuthClaims.SysUserRole, systemUser.Role.Name),
                new Claim(AuthClaims.SysUserRoleId, systemUser.Role.Id.ToString()),
                new(JwtRegisteredClaimNames.Aud, _configuration.GetSection("JWTSetting").GetSection("ValidAudience").Value!),
                new (JwtRegisteredClaimNames.Iss,_configuration.GetSection("JWTSetting").GetSection("ValidIssuer").Value!)
            ];

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(Convert.ToInt32(_configuration.GetSection("JWTSetting").GetSection("ExpireInMinutes").Value!)),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256
                )
            };
            return new JwtSecurityTokenHandler().CreateToken(tokenDescriptor);
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
