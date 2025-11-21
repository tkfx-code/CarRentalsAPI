using CarRentalsClassLibrary.Model;

namespace CarRentalAPI.Interfaces
{
    public interface ICarListingRepo
    {
        //CRUD methods
        Task<IEnumerable<CarListing>> GetAllCarsAsync();
        Task<CarListing?> GetCarByIdAsync(int id);
        Task<CarListing> AddCarAsync(CarListing car);
        Task UpdateCarAsync(CarListing car);
        Task DeleteCarAsync(int id);

        Task<IEnumerable<CarListing>> GetAvailableCarsAsync();

        Task<bool> CarExistsAsync(int id);
    }
}
