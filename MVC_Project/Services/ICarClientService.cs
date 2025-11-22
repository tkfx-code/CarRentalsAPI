using MVC_Project.Models;

namespace MVC_Project.Services
{
    public interface ICarClientService
    {
        Task<Response<List<CarListingViewModel>>> GetAllCarsAsync();
        Task<Response<CarListingViewModel?>> GetCarDetailsAsync(int id);
        Task<bool> CreateCarAsync(CarListingViewModel model);
        Task<bool> UpdateCarAsync(CarListingViewModel model);
        Task<bool> DeleteCarAsync(int id);
    }
}
