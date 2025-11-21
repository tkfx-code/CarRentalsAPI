using CarRentalsClassLibrary.Model;

namespace CarRentalAPI.Interfaces
{
    public interface ICustomerRepo
    {
        //CRUD methods
        Task<IEnumerable<Customer>> GetAllCustomersAsync();
        Task<Customer?> GetCustomerByIdAsync(string id);
        Task<Customer> AddCustomerAsync(Customer customer);
        Task UpdateCustomerAsync(Customer customer);
        Task DeleteCustomerAsync(string id);

        Task<bool> CustomerExistsAsync(string id);

        //View profile task? 
    }
}
