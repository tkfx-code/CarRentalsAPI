using CarRentalAPI.Data;
using CarRentalAPI.Interfaces;
using CarRentalsClassLibrary.Model;
using Microsoft.EntityFrameworkCore;

namespace CarRentalAPI.Repositories
{
    public class CustomerRepo : ICustomerRepo
    {
        private readonly ApplicationDbContext _context;

        // Injektion av databaskontexten
        public CustomerRepo(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Customer>> GetAllCustomersAsync()
        {
            return await _context.Customers.ToListAsync();
        }

        public async Task<Customer?> GetCustomerByIdAsync(string id)
        {
            return await _context.Customers.FindAsync(id);
        }

        public async Task<Customer> AddCustomerAsync(Customer customer)
        {
            await _context.Customers.AddAsync(customer);
            await _context.SaveChangesAsync();
            return customer;
        }

        public async Task UpdateCustomerAsync(Customer customer)
        {
            _context.Customers.Update(customer);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteCustomerAsync(string id)
        {
            var customer = await _context.Customers.FindAsync(id);
            if (customer != null)
            {
                _context.Customers.Remove(customer);
                await _context.SaveChangesAsync();
            }
        }
        public async Task<bool> CustomerExistsAsync(string id)
        {
            return await _context.Customers.AnyAsync(c => c.Id == id);
        }
    }
}
