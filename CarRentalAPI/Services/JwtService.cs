using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CarRentalAPI.Data;
using CarRentalAPI.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace CarRentalAPI.Services
{
    public class JwtService : IAuthService
    {
        private readonly IConfiguration _configuration;

        public JwtService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string CreateJwtToken(APIUser user, IList<string> roles)
        {
            //Define claims
            var claims = new List<Claim>
            {
                // Unique ID for session
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()), 
                //User email
                new Claim(JwtRegisteredClaimNames.Sub, user.Email!), 
                //Users unique ID
                new Claim(ClaimTypes.NameIdentifier, user.Id)
            };

            //Add user role as claims
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            //Fetch secret key
            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));

            //Create credentials
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            //Create token
            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddDays(7),
                signingCredentials: credentials);

            //Make token string and return
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
