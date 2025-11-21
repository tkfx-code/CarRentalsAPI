using MVC_Project.Models;

namespace MVC_Project.Services
{
    public interface ICustomerClientService
    {
        Task<IEnumerable<CustomerViewModel>> GetAllCustomersAsync(string token);
        Task<CustomerViewModel?> GetCustomerDetailsAsync(string id, string token);
        Task<bool> CreateCustomerAsync(CustomerViewModel model, string token);
        Task<bool> UpdateCustomerAsync(CustomerViewModel model, string token);
        Task<bool> DeleteCustomerAsync(string id, string token);
    }
}
