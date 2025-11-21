namespace CarRentalAPI.Dto
{
    public class CarListingDto 
    {
        public int CarId { get; set; }
        public string Make { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
        public List<string> Pictures { get; set; } = new List<string>();
        public bool IsAvailable { get; set; }
    }
}
