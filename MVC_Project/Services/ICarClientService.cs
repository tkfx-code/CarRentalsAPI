using MVC_Project.Models;

namespace MVC_Project.Services
{
    public interface ICarClientService
    {
        Task<IEnumerable<CarListingViewModel>> GetAllCarsAsync(string token);
        Task<CarListingViewModel?> GetCarDetailsAsync(int id, string token);
        Task<bool> CreateCarAsync(CarListingViewModel model, string token);
        Task<bool> UpdateCarAsync(CarListingViewModel model, string token);
        Task<bool> DeleteCarAsync(int id, string token);
    }
}
