using AutoMapper;
using MVC_Project.Models;
using MVC_Project.Services;

namespace MVC_Project.Data
{
    public class MappingProfile:Profile
    {
        public MappingProfile()
        {
            CreateMap<CustomerViewModel, CustomerDto>().ReverseMap();
            CreateMap<CarListingDto, CarListingViewModel>().ReverseMap();
            CreateMap<BookingDto, BookingViewModel>().ForMember(model => model.Car, options => options.MapFrom(booking => booking.Car)).ReverseMap();
        }
    }
}
