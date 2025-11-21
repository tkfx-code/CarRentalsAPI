using System.ComponentModel.DataAnnotations;

namespace CarRentalAPI.Dto
{
    public class CreateCarListingDto
    {
        [Required]
        public string Make { get; set; } = string.Empty;
        [Required]
        public string Model { get; set; } = string.Empty;
        public List<string> Pictures { get; set; } = new List<string>();
        public bool isAvailable { get; set; } = true;
    }
}
