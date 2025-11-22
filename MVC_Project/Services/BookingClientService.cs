using System.Net.Http.Headers;
using Microsoft.AspNetCore.Http;
using MVC_Project.Models;
using MVC_Project.Services;
using CarRentalsClassLibrary.Model;
using AutoMapper;

namespace MVC_Project
{
    public class BookingClientService : BaseService, IBookingClientService
    {
        private readonly IMapper _mapper;

        public BookingClientService(IHttpContextAccessor httpContextAccessor, IClient client, IMapper mapper) : base (httpContextAccessor, client)
        {
            _mapper = mapper;
        }


        public async Task<bool> CreateBookingAsync(BookingViewModel model)
        {
            CarryAccessToken();


            try
            {
                await _client.BookingsPOSTAsync(_mapper.Map<BookingDto>(model));
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to create booking.", ex);
            }
        }

        public async Task<bool> UpdateBookingAsync(BookingViewModel model)
        {
            CarryAccessToken();
            var bookingDto = _mapper.Map<BookingDto>(model);

            try
            {
                await _client.BookingsPUTAsync(model.BookingId, bookingDto);
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to update booking.", ex);
            }
        }

        public async Task<bool> DeleteBookingAsync(int id)
        {
            CarryAccessToken();

            try
            {
                await _client.BookingsDELETEAsync(id);
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to delete booking.", ex);
            }
        }

        public async Task<Response<List<BookingViewModel>>> GetAllBookingsAsync()
        {
            {
                CarryAccessToken();

                try
                {
                    var data = await _client.BookingsAllAsync();

                    if (data == null)
                    {
                        return new Response<List<BookingViewModel>> { Success = true, Data = new List<BookingViewModel>() };
                    }
                    //mapping viewmodel
                    var bookingViewModels = data.Select(booking => new BookingViewModel
                    {
                        BookingId = booking.BookingId,
                        CarId = booking.CarId,
                        StartDate = booking.StartDate,
                        EndDate = booking.EndDate,
                        Customer = new CustomerViewModel
                        {
                            CustomerId = booking.Customer.CustomerId,
                            FirstName = booking.Customer.FirstName,
                            LastName = booking.Customer.LastName,
                            Email = booking.Customer.Email,
                            PhoneNumber = booking.Customer.PhoneNumber
                        },
                        Car = new CarListingViewModel
                        {
                            CarId = booking.Car.CarId,
                            Make = booking.Car.Make,
                            Model = booking.Car.Model
                        }
                    }).ToList();

                    return new Response<List<BookingViewModel>>
                    {
                        Data = bookingViewModels,
                        Success = true
                    };
                }
                catch (Exception ex)
                {
                    return new Response<List<BookingViewModel>>
                    {
                        Message = ex.Message,
                        Success = false
                    };
                }
            }
        }


        public async Task<Response<BookingViewModel>> GetBookingDetailsAsync(int id)
        {
            CarryAccessToken();
            try
            {
                var bookingDto = await _client.BookingsGETAsync(id);

                if (bookingDto == null)
                {
                    return new Response<BookingViewModel> { Success = false, Message = $"Bil ID: {id} could not be found." };
                }

                var viewModel = _mapper.Map<BookingViewModel>(bookingDto);

                return new Response<BookingViewModel> { Data = viewModel, Success = true };
            }
            catch (ApiException ex)
            {
                return new Response<BookingViewModel>
                {
                    Success = false,
                    Message = $"API-error when fetching car: {ex.StatusCode}"
                };
            }
            catch (Exception ex)
            {
                return new Response<BookingViewModel>
                {
                    Success = false,
                    Message = $"An unexpected error occured: {ex.Message}"
                };
            }
        }

        public async Task<Response<BookingViewModel>> GetBookingByIdAsync(int id, BookingViewModel viewModel)
        {
            CarryAccessToken();

            try
            {
                var dto = await _client.BookingsGETAsync(id);

                // Mapping to ViewModel
                var bookingViewModel = new BookingViewModel
                {
                    BookingId = dto.BookingId,
                    CarId = dto.CarId,
                    StartDate = dto.StartDate,
                    EndDate = dto.EndDate
                };

                return new Response<BookingViewModel> { Data = bookingViewModel, Success = true };
            }
            catch (Exception ex)
            {
                return new Response<BookingViewModel> { Success = false, Message = "Not implemented." };
            }
        }
    }
}
