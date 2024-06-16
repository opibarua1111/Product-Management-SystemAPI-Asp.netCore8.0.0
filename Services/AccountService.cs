using Azure.Core;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using ProductManagementAPI.IServices;
using ProductManagementAPI.Models;
using ProductManagementAPI.Models.RequestModels;
using ProductManagementAPI.Models.ResponseModels;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ProductManagementAPI.Services
{
    public class AccountService : IAccountService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AccountService(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IConfiguration configuration,
            IHttpContextAccessor httpContextAccessor
           )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<RegistrationResponse> Register(RegisterModel model)
        {
            try
            {
                var user = new ApplicationUser { UserName = model.UserName, Email = model.Email, EmailConfirmed = true };
                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    return new RegistrationResponse { Success = true, Message = "User created successfully" };
                }

                return new RegistrationResponse { Success = false, Message = "User creation failed", Errors = result.Errors };
            }
            catch(Exception ex)
            {
                throw;
            }
        }
        public async Task<LoginResponse> Login(LoginModel model)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null)
                {
                    return new LoginResponse { Success = false, Message = "User can't find." };
                }

                var result = await _signInManager.PasswordSignInAsync(user.UserName, model.Password, false, false);

                if (result.Succeeded)
                {
                    var loginResult = await LoginSuccess(user);
                    return new LoginResponse { Success = true, Data = loginResult };
                }
                return new LoginResponse { Success = false, Message = "Log in failed"};

            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<object> LoginSuccess(ApplicationUser user)
        {
            object ret;
            try
            {
                var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetSection("AppSettings")["Token"]));
                var signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256Signature);

                var requestHost = _httpContextAccessor.HttpContext.Request.Host.Value;

                var tokeOptions = new JwtSecurityToken(
                            issuer: requestHost,
                            audience: requestHost,
                            claims: new List<Claim>(
                                new List<Claim> {
                                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                                new Claim("userName", user.UserName),
                                new Claim("email", user.Email),
                                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                                new Claim(ClaimTypes.NameIdentifier, user.Id),
                                }
                            ),
                            expires: DateTime.Now.AddDays(1),
                            signingCredentials: signinCredentials
                );

                var tokenHandler = new JwtSecurityTokenHandler();
                ret = new
                {
                    token = tokenHandler.WriteToken(tokeOptions),
                    id = user.Id,
                    name = user.UserName,
                };
                return ret;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
