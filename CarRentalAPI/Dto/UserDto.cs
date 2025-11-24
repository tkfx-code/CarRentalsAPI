using RequiredAttribute = System.ComponentModel.DataAnnotations.RequiredAttribute;

namespace CarRentalAPI.Dto
{
    public class UserDto : LoginUserDto
    {
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
    }
}
