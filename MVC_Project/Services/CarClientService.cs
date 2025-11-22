using AutoMapper;
using MVC_Project.Models;
using MVC_Project.Services;
using CarRentalsClassLibrary.Model;
using Newtonsoft.Json;

namespace MVC_Project.Services
{
    public class CarClientService : BaseService, ICarClientService
    {
        private readonly IMapper _mapper;

        public CarClientService(IHttpContextAccessor httpContextAccessor, IClient client, IMapper mapper) : base(httpContextAccessor, client)
        {
            _mapper = mapper;
        }

        public async Task<bool> CreateCarAsync(CarListingViewModel model)
        {
            var carDto = _mapper.Map<CarListingDto>(model);
            CarryAccessToken();

            try
            {
                await _client.CarListingsPOSTAsync(carDto);
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to create booking.", ex);
            }
        }

        public async Task<bool> DeleteCarAsync(int id)
        {
            CarryAccessToken();
            try
            {
                await _client.CarListingsDELETEAsync(id);
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to delete booking.", ex);
            }
        }

        public async Task<Response<List<CarListingViewModel>>> GetAllCarsAsync()
        {
            CarryAccessToken();
            try
            {
                var data = await _client.CarListingsAllAsync();

                if (data == null)
                {
                    return new Response<List<CarListingViewModel>> { Success = true, Data = new List<CarListingViewModel>() };
                }

                //mapping viewmodel
                var carListingViewModels = data.Select(car => new CarListingViewModel
                {
                    CarId = car.CarId,
                    Make = car.Make,
                    Model = car.Model,
                    isAvailable = car.IsAvailable
                }).ToList();

                return new Response<List<CarListingViewModel>>
                {
                    Data = carListingViewModels,
                    Success = true
                };
            }
            catch (Exception ex)
            {
                return new Response<List<CarListingViewModel>>
                {
                    Message = ex.Message,
                    Success = false
                };
            }
        }

        public async Task<Response<CarListingViewModel?>> GetCarDetailsAsync(int id)
        {
            CarryAccessToken();
            try
            {
                var carDto = await _client.CarListingsGETAsync(id);

                if (carDto == null)
                {
                    return new Response<CarListingViewModel?> { Success = false, Message = $"Bil ID: {id} could not be found." };
                }

                var viewModel = _mapper.Map<CarListingViewModel>(carDto);

                return new Response<CarListingViewModel?> { Data = viewModel, Success = true };
            }
            catch (ApiException ex)
            {
                return new Response<CarListingViewModel?>
                {
                    Success = false,
                    Message = $"API-error when fetching car: {ex.StatusCode}"
                };
            }
            catch (Exception ex)
            {
                return new Response<CarListingViewModel?>
                {
                    Success = false,
                    Message = $"An unexpected error occured: {ex.Message}"
                };
            }
        }

        public async Task<bool> UpdateCarAsync(CarListingViewModel model)
        {
            CarryAccessToken();
            if (model.CarId <= 0)
            {
                throw new ArgumentException("Could not find car to update.");
            }

            var carDto = _mapper.Map<CarListingDto>(model);

            try
            {
                
                await _client.CarListingsPUTAsync(model.CarId, carDto);

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Kunde inte uppdatera bil med ID {model.CarId}.", ex);
            }
        }
    }
}
