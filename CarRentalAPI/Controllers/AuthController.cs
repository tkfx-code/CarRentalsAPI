using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CarRentalAPI.Data;
using CarRentalAPI.Dto;
using CarRentalAPI.Interfaces;
using CarRentalsClassLibrary.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace CarRentalAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<APIUser> _userManager;
        private readonly IConfiguration configuration;
        private readonly ICustomerRepo customerRepo;

        public AuthController(UserManager<APIUser> userManager, IConfiguration configuration, ICustomerRepo customerRepo)
        {
            _userManager = userManager;
            this.configuration = configuration;
            this.customerRepo = customerRepo;
        }

        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody] UserDto userDto)
        {
            try
            {
                var user = new APIUser
                {
                    UserName = userDto.Email,
                    Email = userDto.Email,
                    EmailConfirmed = true
                };
                var result = await _userManager.CreateAsync(user, userDto.Password);

                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(error.Code, error.Description);
                    }
                    return BadRequest(ModelState);
                }

                //add to role and create Customer entry
                await _userManager.AddToRoleAsync(user, "User");

                var customer = new Customer
                {
                    Id = user.Id,
                    FirstName = userDto.FirstName,
                    LastName = userDto.LastName,
                    Email = userDto.Email,
                    PhoneNumber = userDto.PhoneNumber
                };

                //save customer to db
                await customerRepo.AddCustomerAsync(customer);

                //return Jwt token
                string jwttoken = await CreateToken(user);
                var response = new AuthResponseDto
                {
                    TokenString = jwttoken,
                    UserId = user.Id,
                    Email = userDto.Email
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return Problem($"Something went wrong in the {ex}", statusCode: 500);
            }
        }

        [HttpPost]
        [Route("login")]
        public async Task<ActionResult<AuthResponseDto>> Login([FromBody] LoginUserDto loginUserDto)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(loginUserDto.Email);
                var passwordValid = await _userManager.CheckPasswordAsync(user, loginUserDto.Password);

                if (user == null || passwordValid == false)
                {
                    return Unauthorized(loginUserDto);
                }

                string jwttoken = await CreateToken(user);
                var response = new AuthResponseDto
                {
                    TokenString = jwttoken,
                    UserId = user.Id,
                    Email = loginUserDto.Email
                };
                return Ok(response);
            }
            catch (Exception ex)
            {
                return Problem($"Something went wrong in the {ex}", statusCode: 500);
            }
        }

        //Fix Custom Claim types and User roles
        private async Task<string> CreateToken(APIUser user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JwtSettings:Key"]!));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var roles = await _userManager.GetRolesAsync(user);
            var roleClaims = roles.Select(role => new Claim(ClaimTypes.Role, role)).ToList();
            var userClaims = await _userManager.GetClaimsAsync(user);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Email!),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email!),
                new Claim(ClaimTypes.NameIdentifier, user.Id)
            }
            .Union(roleClaims)
            .Union(userClaims);

            var token = new JwtSecurityToken(
                issuer: configuration["JwtSettings:Issuer"],
                audience: configuration["JwtSettings:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(3),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
