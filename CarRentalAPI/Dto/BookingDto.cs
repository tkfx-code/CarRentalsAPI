using System.Text.Json.Serialization;

namespace CarRentalAPI.Dto
{
    public class BookingDto
    {
        public int BookingId { get; set; }
        public string CustomerId { get; set; } = "";
        public int CarId { get; set; }
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }

        [JsonIgnore]
        public CustomerDto Customer { get; set; } = null!;
        [JsonIgnore]
        public CarListingDto Car { get; set; } = null!;
    }
}
