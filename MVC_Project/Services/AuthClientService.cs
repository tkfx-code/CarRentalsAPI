using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
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
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;

        //Internal DTOs so I don't have to move all DTOs to Shared Class Library at this point
        private record ApiLoginDto(string Email, string Password);
        private record ApiRegisterDto(string Email, string Password, string Username);

        public AuthClientService(HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = httpClient;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<bool> LoginAsync(LoginViewModel model)
        {
            var apiLoginDto = new ApiLoginDto(model.Email, model.Password);
            var response = await _httpClient.PostAsJsonAsync("api/auth/login", apiLoginDto);

            if (!response.IsSuccessStatusCode)
            {
                return false;
            }

            var authResponse = await response.Content.ReadFromJsonAsync<AuthResponseDto>();
            if (authResponse == null)
            {
                return false;
            }
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, authResponse.UserId),
                new Claim(ClaimTypes.Email, authResponse.Email),
                new Claim("Token", authResponse.TokenString)
            };

            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(authResponse.TokenString);

            var roleClaims = jwtToken.Claims.Where(c => c.Type == ClaimTypes.Role);
            claims.AddRange(roleClaims);
            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            await _httpContextAccessor.HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity)
                );
            return true;
        }

        public async Task Logout()
        {
            //remove authentication cookie
            await _httpContextAccessor.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        }

        public async Task<bool> RegisterAsync(RegisterViewModel model)
        {
            //map from ViewModel to internal DTO 
            var apiRegisterDto = new ApiRegisterDto(
                model.Email,
                model.Password,
                model.Email //using email as username for simplicity
                );

            var response = await _httpClient.PostAsJsonAsync("api/auth/register", apiRegisterDto);

            return response.IsSuccessStatusCode;
        }
    }

}
