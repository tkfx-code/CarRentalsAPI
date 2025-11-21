using System.ComponentModel.DataAnnotations;

namespace CarRentalAPI.Dto
{
    public class LoginUserDto /*: IdentityUser*/
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
