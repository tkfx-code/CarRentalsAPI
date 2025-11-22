using MVC_Project.Models;

namespace MVC_Project.Services
{
    public interface ICustomerClientService
    {
        Task<Response<List<CustomerViewModel>>> GetAllCustomersAsync();
        Task<Response<CustomerViewModel?>> GetCustomerDetailsAsync(string id);
        Task<bool> CreateCustomerAsync(CustomerViewModel model);
        Task<bool> UpdateCustomerAsync(CustomerViewModel model);
        Task<bool> DeleteCustomerAsync(string id);
    }
}
