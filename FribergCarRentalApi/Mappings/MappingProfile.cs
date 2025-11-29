using AutoMapper;
using FribergCarRental.Core.Classes;
using FribergCarRental.Core.Dtos;


namespace FribergCarRental.Api.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Car, CarDto>().ReverseMap();
            CreateMap<Booking, BookingDto>().ReverseMap();
            CreateMap<EditBookingDto, Booking>();
            CreateMap<EditApplicationUserDto, ApplicationUserDto>();
            CreateMap<Image, ImageDto>().ReverseMap();
            CreateMap<ApplicationUser, ApplicationUserDto>().ReverseMap();
            CreateMap<CreateApplicationUserDto, ApplicationUserDto>();
            CreateMap<EditApplicationUserDto, ApplicationUser>();
        }

    }
}
