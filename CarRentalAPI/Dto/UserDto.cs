using RequiredAttribute = System.ComponentModel.DataAnnotations.RequiredAttribute;

namespace CarRentalAPI.Dto
{
    public class UserDto : LoginUserDto
    {
        [Required]
        public string UserName { get; set; } 
        //[Required]
        //[EmailAddress]
        //public string Email { get; set; }
        //[Required]
        //public string Password { get; set; }
    }
}
