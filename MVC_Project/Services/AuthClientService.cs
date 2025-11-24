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

            //jwt saved in session for API
            _httpContextAccessor.HttpContext.Session.SetString("JWTtoken", response.TokenString);

            //create ClaimsIdentity for local cookie auth
            var claims = new List<Claim>();

            //fetch UserID and email from JWT
            var idClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            var emailClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email);

            if (idClaim != null) claims.Add(idClaim);
            if (emailClaim != null) claims.Add(emailClaim);

            //add to role claims
            var roleClaims = jwtToken.Claims.Where(c => c.Type == ClaimTypes.Role || c.Type == "role");
            claims.AddRange(roleClaims);

            //add token as specialclaim for BaseService
            claims.Add(new Claim("Token", response.TokenString));

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            //create local auth cookie
            await _httpContextAccessor.HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity));

            return true;
        }

        public async Task Logout()
        {
            _httpContextAccessor.HttpContext.Session.Remove("JWTtoken");
            await Task.CompletedTask;
            //remove authentication cookie
            await _httpContextAccessor.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        }

        public async Task<bool> RegisterAsync(RegisterViewModel model)
        {
            //map from ViewModel to internal DTO 
            var apiRegisterDto = new UserDto
            {
                Email = model.Email,
                Password = model.Password,
            };

            try
            {
                await _client.RegisterAsync(apiRegisterDto);

                //log in after register
                var loginDto = new LoginUserDto { Email = model.Email, Password = model.Password };
                return await LoginAsync(loginDto);
            }
            catch (ApiException ex)
            {
                Console.WriteLine($"API exception> {ex.StatusCode} - {ex.Response}");
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception registration: {ex.Message}");
                return false;
            }
        }
    }
}
