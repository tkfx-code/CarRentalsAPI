using AutoMapper;
using CarRentalAPI.Dto;
using CarRentalsClassLibrary.Model;

namespace MVC_Project.Data
{
    public class ApiMappingProfile:Profile
    {
        public ApiMappingProfile()
        {
            CreateMap<Customer, CustomerDto>().ReverseMap();
            CreateMap<CarListing, CarListingDto>().ReverseMap();
            CreateMap<Booking, BookingDto>().ForMember(model => model.Car, options => options.MapFrom(booking => booking.Car)).ReverseMap();
            CreateMap<Booking, CreateBookingDto>().ReverseMap();
        }
    }
}
