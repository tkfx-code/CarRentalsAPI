using CarRentalAPI.Data;
using CarRentalAPI.Interfaces;
using CarRentalsClassLibrary.Model;
using Microsoft.EntityFrameworkCore;

namespace CarRentalAPI.Repositories
{
    public class CarListingRepo : ICarListingRepo
    {
        private readonly ApplicationDbContext _context;

        // Injektion av databaskontexten
        public CarListingRepo(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<CarListing>> GetAllCarsAsync()
        {
            return await _context.CarListings.ToListAsync();
        }

        public async Task<CarListing?> GetCarByIdAsync(int id)
        {
            return await _context.CarListings.FindAsync(id);
        }

        public async Task<CarListing> AddCarAsync(CarListing car)
        {
            _context.CarListings.Add(car);
            await _context.SaveChangesAsync();
            return car;
        }

        public async Task UpdateCarAsync(CarListing car)
        {
            _context.CarListings.Update(car);
            // Märk: Update-metoden för DbContext spårar ändringarna.
            // För att hantera bilder/listor korrekt vid Edit behöver vi kanske en annan metod, men detta är grunden.
            await _context.SaveChangesAsync();
        }

        public async Task DeleteCarAsync(int id)
        {
            var car = await _context.CarListings.FindAsync(id);
            if (car != null)
            {
                _context.CarListings.Remove(car);
                await _context.SaveChangesAsync();
            }
        }

        // Specifik affärslogik
        public async Task<IEnumerable<CarListing>> GetAvailableCarsAsync()
        {
            return await _context.CarListings
                .Where(c => c.isAvailable)
                .ToListAsync();
        }

        public Task<bool> CarExistsAsync(int id)
        {
            return _context.CarListings.AnyAsync(e => e.CarId == id);
        }
    }
}
