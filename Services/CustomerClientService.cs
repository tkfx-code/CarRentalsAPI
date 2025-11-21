using MVC_Project.Models;

namespace MVC_Project.Services
{
    public class CustomerClientService : ICustomerClientService
    {
        public Task<bool> CreateCustomerAsync(CustomerViewModel model, string token)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteCustomerAsync(string id, string token)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<CustomerViewModel>> GetAllCustomersAsync(string token)
        {
            throw new NotImplementedException();
        }

        public Task<CustomerViewModel?> GetCustomerDetailsAsync(string id, string token)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateCustomerAsync(CustomerViewModel model, string token)
        {
            throw new NotImplementedException();
        }
    }
}
