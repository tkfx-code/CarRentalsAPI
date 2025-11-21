namespace CarRentalAPI.Dto
{
    public class BookingDto
    {
        public int BookingId { get; set; }
        public string CustomerId { get; set; } = "";
        public int CarId { get; set; }
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }

        public CustomerDto Customer { get; set; }
        public CarListingDto Car { get; set; }
    }
}
