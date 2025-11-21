using MVC_Project.Models;

namespace MVC_Project.Services
{
    public class CarClientService : ICarClientService
    {
        public Task<bool> CreateCarAsync(CarListingViewModel model, string token)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteCarAsync(int id, string token)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<CarListingViewModel>> GetAllCarsAsync(string token)
        {
            throw new NotImplementedException();
        }

        public Task<CarListingViewModel?> GetCarDetailsAsync(int id, string token)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateCarAsync(CarListingViewModel model, string token)
        {
            throw new NotImplementedException();
        }
    }
}
