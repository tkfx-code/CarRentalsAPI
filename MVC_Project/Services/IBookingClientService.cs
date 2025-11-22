using MVC_Project.Models;
using MVC_Project.Services;

namespace MVC_Project.Services
{
    public interface IBookingClientService
    {
        //Returns ViewModels
        Task<Response<List<BookingViewModel>>> GetAllBookingsAsync();
        Task<Response<BookingViewModel>> GetBookingDetailsAsync(int id);
        Task<bool> CreateBookingAsync(BookingViewModel model);
        Task<bool> DeleteBookingAsync(int id);
        Task<bool> UpdateBookingAsync(BookingViewModel model);

    }
}
