using System.ComponentModel.DataAnnotations;

namespace CarRentalsClassLibrary.Model
{
    public class Customer
    {
        public string Id { get; set; }
        [Required(ErrorMessage = "First Name is required")]
        public string FirstName { get; set; } = "";
        [Required(ErrorMessage = "Last Name is required")]
        public string LastName { get; set; } = "";
        [Required]
        public string Email { get; set; } = "";
        //setting phonenumber to string since int would not allow leading zeros
        //Which will make ModelState be invalid and return fals, and model binding to silently fail.
        public string PhoneNumber { get; set; } = string.Empty;
        //public string? UserId { get; set; } //Foreign key to IdentityUser

        //One to many : One customer can have many bookings
        public virtual List<Booking> Bookings { get; set; }
    }
}
