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
            CreateMap<Image, ImageDto>().ReverseMap();
            CreateMap<ApplicationUser, ApplicationUserDto>().ReverseMap();
            CreateMap<ApplicationUser, CreateApplicationUserDto>()
                .ReverseMap()
                .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
                .ForMember(dest => dest.SecurityStamp, opt => opt.Ignore())
            .ForSourceMember(src => src.Password, opt => opt.DoNotValidate());
        }

    }
}
