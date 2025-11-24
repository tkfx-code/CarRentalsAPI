using System.ComponentModel.DataAnnotations;

namespace CarRentalAPI.Dto
{
    public class CustomerDto
    {
        public string CustomerId { get; set; } = "";
        public string FirstName { get; set; } = "";
        public string LastName { get; set; } = "";
        [Required]
        [EmailAddress]
        public string Email { get; set; } = "";
        public string PhoneNumber { get; set; } = string.Empty;
    }
}
