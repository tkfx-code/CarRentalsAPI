namespace CarRentalAPI.Dto
{
    public class CreateBookingDto
    {
        public int BookingId { get; set; }
        public string CustomerId { get; set; } = "";
        public int CarId { get; set; }
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
    }
}
