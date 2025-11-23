using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Owin.Security.Provider;
using MVC_Project.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MVC_Project.Services
{
    public class AuthClientService : IAuthClientService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IClient _client;

        public AuthClientService(IClient client, IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            _client = client;
        }

        public async Task<bool> LoginAsync(LoginUserDto loginmodel)
        {
            var response = await _client.LoginAsync(loginmodel);


            if (response == null || string.IsNullOrEmpty(response.TokenString))
            {
                return false;
            }

            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(response.TokenString) as JwtSecurityToken;

            var claims = jwtToken.Claims
                .Where(c => c.Type == ClaimTypes.Role || c.Type == "role")
                .Select(c => c.Value)
                .ToList();

            _httpContextAccessor.HttpContext.Session.SetString("JWToken", response.TokenString);

            if (claims != null && claims.Any())
            {
                _httpContextAccessor.HttpContext.Session.SetString("UserRoles", System.Text.Json.JsonSerializer.Serialize(claims));
            }

            return true;
        }

            //{
            //    new Claim(ClaimTypes.NameIdentifier, response.UserId),
            //    new Claim(ClaimTypes.Email, response.Email),
            //    new Claim("Token", response.TokenString)
            //};


            //    var roleClaims = jwtToken.Claims.Where(c => c.Type == ClaimTypes.Role);
            //    claims.AddRange(roleClaims);

            //    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            //    await _httpContextAccessor.HttpContext.SignInAsync(
            //        CookieAuthenticationDefaults.AuthenticationScheme,
            //        new ClaimsPrincipal(claimsIdentity)
            //    );
            //    return true;
            //}

        public async Task Logout()
        {
            _httpContextAccessor.HttpContext.Session.Remove("JWToken");
            _httpContextAccessor.HttpContext.Session.Remove("UserRoles");
            await Task.CompletedTask;
            //remove authentication cookie
            //await _httpContextAccessor.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        }

        public async Task<bool> RegisterAsync(RegisterViewModel model)
        {
            //map from ViewModel to internal DTO 
            var apiRegisterDto = new UserDto
            {
                Email = model.Email,
                Password = model.Password,
                UserName = model.ConfirmPassword
            };

            try
            {
                await _client.RegisterAsync(apiRegisterDto);
                return true;
            }
            catch (ApiException ex)
            {
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
