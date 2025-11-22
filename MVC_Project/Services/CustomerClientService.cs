using AutoMapper;
using CarRentalsClassLibrary.Model;
using MVC_Project.Models;
using Newtonsoft.Json;


namespace MVC_Project.Services
{
    public class CustomerClientService : BaseService, ICustomerClientService
    {
        private readonly IMapper _mapper;

        public CustomerClientService(IHttpContextAccessor httpContextAccessor, IClient client, IMapper mapper) : base(httpContextAccessor, client)
        {
            _mapper = mapper;
        }

        public async Task<bool> CreateCustomerAsync(CustomerViewModel model)
        {
            var customerDto = _mapper.Map<CustomerDto>(model);
            CarryAccessToken();

            try
            {
                await _client.CustomersPOSTAsync(customerDto);
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to create booking.", ex);
            }
        }

        public async Task<bool> DeleteCustomerAsync(string id)
        {
            CarryAccessToken();
            try
            {
                await _client.CustomersDELETEAsync(id);
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to delete booking.", ex);
            }
        }

        public async Task<Response<List<CustomerViewModel>>> GetAllCustomersAsync()
        {
            CarryAccessToken();
            try
            {
                var data = await _client.CustomersAllAsync();

                if (data == null)
                {
                    return new Response<List<CustomerViewModel>> { Success = true, Data = new List<CustomerViewModel>() };
                }

                //mapping viewmodel
                var customerViewModels = data.Select(customer => new CustomerViewModel
                {
                    CustomerId = customer.CustomerId,
                    FirstName = customer.FirstName,
                    LastName = customer.LastName,
                    Email = customer.Email,
                    PhoneNumber = customer.PhoneNumber

                }).ToList();

                return new Response<List<CustomerViewModel>>
                {
                    Data = customerViewModels,
                    Success = true
                };
            }
            catch (Exception ex)
            {
                return new Response<List<CustomerViewModel>>
                {
                    Message = ex.Message,
                    Success = false
                };
            }
        }

        public async Task<Response<CustomerViewModel?>> GetCustomerDetailsAsync(string id)
        {
            CarryAccessToken();
            try
            {
                var customerDto = await _client.CustomersGETAsync(id);

                if (customerDto == null)
                {
                    return new Response<CustomerViewModel?> { Success = false, Message = $"Customer could not be found." };
                }

                var customerViewModel = _mapper.Map<CustomerViewModel>(customerDto);

                return new Response<CustomerViewModel?> { Data = customerViewModel, Success = true };
            }
            catch (ApiException ex)
            {
                return new Response<CustomerViewModel?>
                {
                    Success = false,
                    Message = $"API-error when fetching car: {ex.StatusCode}"
                };
            }
            catch (Exception ex)
            {
                return new Response<CustomerViewModel?>
                {
                    Success = false,
                    Message = $"An unexpected error occured: {ex.Message}"
                };
            }
        }

        public Task<bool> UpdateCustomerAsync(CustomerViewModel model)
        {
            throw new NotImplementedException();
        }
    }
}
