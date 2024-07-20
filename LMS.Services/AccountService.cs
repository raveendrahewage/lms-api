using AutoMapper;
using Azure;
using LMS.Api.ViewModels;
using LMS.Data;
using LMS.Data.Enum;
using LMS.Data.Models;
using LMS.Services.Interfaces;
using LMS.Services.Responses;
using LMS.Services.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
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
    public class AccountService(
        ApplicationDbContext appDbContext,
        SignInManager<SystemUser> signInManager,
        UserManager<SystemUser> userManager,
        IConfiguration configuration,
        IMapper mapper,
        IHttpContextAccessor httpContextAccessor
        ) : IAccountService
    {
        private readonly ApplicationDbContext _appDbContext = appDbContext;
        private readonly SignInManager<SystemUser> _signInManager = signInManager;
        private readonly UserManager<SystemUser> _userManager = userManager;
        private readonly IConfiguration _configuration = configuration;
        private readonly IMapper _mapper = mapper;
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

        public async Task<AuthResult> SignIn(SignInViewModel model)
        {
            try
            {
                var signInResult = await SignInAsync(model, false);
                if (signInResult)
                {
                    var sysUser = await _userManager.FindByEmailAsync(model.Email);
                    if(sysUser is not null)
                    {
                        var systemUser = await _appDbContext.SystemUsers
                            .Include(x => x.Role)
                            .FirstAsync(x => x.Id == sysUser.Id);
                        var systemUserViewModel = _mapper.Map<SystemUser, SystemUserViewModel>(systemUser);
                        var token = GenerateToken(systemUser);
                        return new AuthResult(
                            new JwtSecurityTokenHandler().WriteToken(token),
                            token.ValidTo,
                            systemUserViewModel
                        );
                    }
                }
                throw new Exception("Signed in failed! Incorrect username or password.");
            } catch(Exception)
            {
                throw;
            }
        }

        public async Task<AuthResult> SignUp(SignUpViewModel model)
        {
            try
            {
                var signUpResult = await SignUpAsync(model);
                if (signUpResult)
                {
                    var sysUser = await _userManager.FindByEmailAsync(model.Email);
                    if (sysUser is not null)
                    {
                        var systemUser = await _appDbContext.SystemUsers
                            .Include(x => x.Role)
                            .FirstAsync(x => x.Id == sysUser.Id);
                        var systemUserViewModel = _mapper.Map<SystemUser, SystemUserViewModel>(systemUser);
                        var token = GenerateToken(systemUser);
                        return new AuthResult(
                            new JwtSecurityTokenHandler().WriteToken(token),
                            token.ValidTo,
                            systemUserViewModel
                        );
                    }
                }
                throw new Exception("Signed up failed! Incorrect username or password.");
            }
            catch(Exception)
            {
                throw;
            }
        }
        public async Task<bool> ResetPassword(ResetPasswordViewModel model)
        {
            try
            {
                var loggedInSystemUser = await _userManager.FindByEmailAsync(GetCurrentLoggedInUsername());
                if(loggedInSystemUser is not null)
                {
                    var passwordCheckResult = await _userManager.CheckPasswordAsync(loggedInSystemUser, model.OldPassword);
                    if (passwordCheckResult)
                    {
                        model.Token = string.IsNullOrEmpty(model.Token) ? await _userManager.GeneratePasswordResetTokenAsync(loggedInSystemUser): model.Token;
                        var result = await _userManager.ResetPasswordAsync(loggedInSystemUser, model.Token, model.NewPassword);
                        if (result is not null && result.Succeeded)
                            return result.Succeeded;
                        throw new Exception("Password reset failed! Something went wrong.");
                    }
                    throw new Exception("Incorrect password!");
                }
                throw new Exception("System user not found!");
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<string> GetPasswordResetToken()
        {
            try
            {
                var loggedInSystemUser = await _userManager.FindByEmailAsync(GetCurrentLoggedInUsername());
                if (loggedInSystemUser is not null)
                {
                    return await _userManager.GeneratePasswordResetTokenAsync(loggedInSystemUser);
                }
                throw new Exception("System user not found!");
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<SystemUserViewModel> GetLoggedInSystemUser()
        {
            try
            {
                var loggedInSystemUser = await _userManager.FindByEmailAsync(GetCurrentLoggedInUsername());
                if (loggedInSystemUser is not null)
                {
                    var systemUser = await _appDbContext.SystemUsers
                        .Include(x => x.Role)
                        .Include(x => x.Leaves)
                        .Include(x => x.ReviewedLeaves)
                        .Include(x => x.EmployeesUnderSupervision)
                        .FirstOrDefaultAsync(x => x.Id == loggedInSystemUser.Id);
                    return _mapper.Map<SystemUser, SystemUserViewModel>(loggedInSystemUser);
                }
                throw new Exception("System user not found!");
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<bool> SignInAsync(SignInViewModel model, bool lockoutOnFailure = false)
        {
            try
            {
                var signInResult = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure);
                if (signInResult != null && signInResult.Succeeded) return true;
                return false;
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public async Task<bool> SignUpAsync(SignUpViewModel model)
        {
            try
            {
                var newSystemUser = _mapper.Map<SignUpViewModel, SystemUser>(model);
                newSystemUser.SupervisorId = model.SupervisorId is not null && model.SupervisorId > 0 ? model.SupervisorId : null;
                var signUpResult = await _userManager.CreateAsync(newSystemUser, model.Password);
                if (signUpResult != null && signUpResult.Succeeded) return true;
                return false;
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
        private SecurityToken GenerateToken(SystemUser systemUser)
        {
            try
            {
                var key = Encoding.UTF8.GetBytes(_configuration.GetSection("JWTSetting").GetSection("SecurityKey").Value!);
                List<Claim> claims =
                [
                    new Claim(AuthClaim.SysUserUsername, systemUser.Email ?? string.Empty),
                    new Claim(AuthClaim.SysUserUserId, systemUser.Id.ToString()),
                    new Claim(AuthClaim.SysUserRole, systemUser.Role.Name ?? string.Empty),
                    new Claim(AuthClaim.SysUserRoleId, systemUser.Role.Id.ToString()),
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
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
        public string GetCurrentLoggedInUsername() => _httpContextAccessor?.HttpContext?.User?.Claims?.FirstOrDefault(x => x.Type == AuthClaim.SysUserUsername)?.Value ?? string.Empty;
        public int GetCurrentLoggedInUserId() => Convert.ToInt32(_httpContextAccessor?.HttpContext?.User?.Claims?.FirstOrDefault(x => x.Type == AuthClaim.SysUserUserId)?.Value ?? "0");
    }
}
