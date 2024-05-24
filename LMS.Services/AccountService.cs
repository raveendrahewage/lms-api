using AutoMapper;
using Azure;
using LMS.Api.ViewModels;
using LMS.Data;
using LMS.Data.Enum;
using LMS.Data.Models;
using LMS.Services.Helpers.Interfaces;
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
        private readonly ApplicationDbContext _appDbContext;
        private readonly RoleManager<SystemRole> _roleManager;
        private readonly SignInManager<SystemUser> _signInManager;
        private readonly UserManager<SystemUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;
        private readonly IApiResponseHelper _apiResponseHelper;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AccountService(
            ApplicationDbContext appDbContext,
            RoleManager<SystemRole> roleManager,
            SignInManager<SystemUser> signInManager,
            UserManager<SystemUser> userManager,
            IConfiguration configuration,
            IMapper mapper,
            IApiResponseHelper apiResponseHelper,
            IHttpContextAccessor httpContextAccessor
        )
        {
            _appDbContext = appDbContext;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _userManager = userManager;
            _configuration = configuration;
            _mapper = mapper;
            _apiResponseHelper = apiResponseHelper;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<ApiResponse<AuthResult>> SignIn(SignInViewModel model)
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
                            .FirstOrDefaultAsync(x => x.Id == sysUser.Id);
                        var systemUserViewModel = _mapper.Map<SystemUserViewModel>(systemUser);
                        var token = GenerateToken(systemUser);
                        return _apiResponseHelper.GenerateApiResponse<AuthResult>(
                            signInResult,
                            "Signed in successfully!",
                            new AuthResult(
                                new JwtSecurityTokenHandler().WriteToken(token),
                                token.ValidTo,
                                systemUserViewModel
                            )
                        );
                    }
                }
                return new ApiResponse<AuthResult>(signInResult, "Signed in failed! Incorrect username or password.");
            } catch(Exception ex)
            {
                return _apiResponseHelper.GenerateApiResponse<AuthResult>(false, ex.Message);
            }
        }

        public async Task<ApiResponse<AuthResult>> SignUp(SignUpViewModel model)
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
                            .FirstOrDefaultAsync(x => x.Id == sysUser.Id);
                        var systemUserViewModel = _mapper.Map<SystemUserViewModel>(systemUser);
                        var token = GenerateToken(systemUser);
                        return _apiResponseHelper.GenerateApiResponse<AuthResult>(
                            signUpResult,
                            "Signed up successfully!",
                            new AuthResult(
                                new JwtSecurityTokenHandler().WriteToken(token),
                                token.ValidTo,
                                systemUserViewModel
                            )
                        );
                    }
                }
                return _apiResponseHelper.GenerateApiResponse<AuthResult>(signUpResult, "Signed up failed! Incorrect username or password.");
            }
            catch(Exception ex)
            {
                return _apiResponseHelper.GenerateApiResponse<AuthResult>(false, ex.Message);
            }
        }
        public async Task<ApiResponse<bool>> ResetPassword(ResetPasswordViewModel model)
        {
            try
            {
                var loggedInSystemUser = await _userManager.FindByEmailAsync(GetCurrentLoggedInUsername());
                if(loggedInSystemUser is not null)
                {
                    var passwordCheckResult = await _userManager.CheckPasswordAsync(loggedInSystemUser, model.OldPassword);
                    if (passwordCheckResult)
                    {
                        var result = await _userManager.ResetPasswordAsync(loggedInSystemUser, model.Token, model.NewPassword);
                        if(result is not null && result.Succeeded)
                            return _apiResponseHelper.GenerateApiResponse<bool>(result.Succeeded, "Password reset successfully!", result.Succeeded);
                        return _apiResponseHelper.GenerateApiResponse<bool>(false, "Password reset failed! Something went wrong.");
                    }
                    return _apiResponseHelper.GenerateApiResponse<bool>(false, "Incorrect password!");
                }
                return _apiResponseHelper.GenerateApiResponse<bool>(false, "System user not found!");
            }
            catch (Exception ex)
            {
                return _apiResponseHelper.GenerateApiResponse<bool>(false, ex.Message);
            }
        }
        public async Task<ApiResponse<string>> GetPasswordResetToken()
        {
            try
            {
                var loggedInSystemUser = await _userManager.FindByEmailAsync(GetCurrentLoggedInUsername());
                if (loggedInSystemUser is not null)
                {
                    var token = await _userManager.GeneratePasswordResetTokenAsync(loggedInSystemUser);
                    return _apiResponseHelper.GenerateApiResponse<string>(true, token);
                }
                return _apiResponseHelper.GenerateApiResponse<string>(false, "System user not found!");
            }
            catch (Exception ex)
            {
                return _apiResponseHelper.GenerateApiResponse<string>(false, ex.Message);
            }
        }
        public async Task<ApiResponse<SystemUserViewModel>> GetLoggedInSystemUser()
        {
            try
            {
                var loggedInSystemUser = await _userManager.FindByEmailAsync(GetCurrentLoggedInUsername());
                if (loggedInSystemUser is not null)
                {
                    var systemUser = _appDbContext.SystemUsers
                        .Include(x => x.Role)
                        .Include(x => x.Leaves)
                        .Include(x => x.ReviewedLeaves)
                        .FirstOrDefaultAsync(x => x.Id == loggedInSystemUser.Id);
                    var systemUserViewModel = _mapper.Map<SystemUserViewModel>(loggedInSystemUser);
                    return _apiResponseHelper.GenerateApiResponse<SystemUserViewModel>(true, systemUserViewModel);
                }
                return _apiResponseHelper.GenerateApiResponse<SystemUserViewModel>(false, "System user not found!");
            }
            catch (Exception ex)
            {
                return _apiResponseHelper.GenerateApiResponse<SystemUserViewModel>(false, ex.Message);
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
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
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
