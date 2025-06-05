using AutoMapper;
using FribergCarRental.Classes;
using FribergCarRental.Models;

namespace FribergCarRental.Data
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Car, CarViewModel>().ReverseMap();
            CreateMap<Booking, BookingViewModel>().ReverseMap();
            CreateMap<Image, ImageViewModel>().ReverseMap();
            CreateMap<ApplicationUser, UserViewModel>()
                .ReverseMap()
                .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
                .ForMember(dest => dest.SecurityStamp, opt => opt.Ignore())
                .ForSourceMember(src => src.NewPassword, opt => opt.DoNotValidate());
        }
    }
}
