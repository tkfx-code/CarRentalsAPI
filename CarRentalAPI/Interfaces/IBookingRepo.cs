using CarRentalsClassLibrary.Model;

namespace CarRentalAPI.Interfaces
{
    public interface IBookingRepo
    {
        //CRUD methods
        Task<IEnumerable<Booking>> GetAllBookingsAsync();
        Task<Booking?> GetBookingByIdAsync(int id);
        Task<Booking> AddBookingAsync(Booking booking);
        Task<bool> UpdateBookingAsync(Booking booking);
        Task DeleteBookingAsync(int id);

        Task<bool> BookingExistsAsync(int id);
        Task<IEnumerable<Booking>> GetBookingsByCustomerIdAsync(string customerId);
    }
}
