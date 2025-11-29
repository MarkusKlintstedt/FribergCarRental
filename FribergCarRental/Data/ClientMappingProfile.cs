using AutoMapper;
using FribergCarRental.Models;
using FribergCarRental.Services.Base;

namespace FribergCarRental.Data
{
    public class ClientMappingProfile : Profile
    {
        public ClientMappingProfile()
        {
            CreateMap<CarDto, CarViewModel>().ReverseMap();
            CreateMap<BookingDto, BookingViewModel>().ReverseMap();
            CreateMap<BookingViewModel, EditBookingDto>();
            CreateMap<ImageDto, ImageViewModel>().ReverseMap();
            CreateMap<CreateUserViewModel, CreateApplicationUserDto>();
            CreateMap<UserViewModel, EditApplicationUserDto>();
            CreateMap<ApplicationUserDto, UserViewModel>().ReverseMap();
        }
    }
}
