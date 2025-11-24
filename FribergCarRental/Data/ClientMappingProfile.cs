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
            CreateMap<CreateUserViewModel, CreateApplicationUserDto>().ReverseMap();
            CreateMap<UserViewModel, EditApplicationUserDto>();

            CreateMap<ApplicationUserDto, UserViewModel>()
                .ReverseMap();
            //.ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
            //.ForMember(dest => dest.SecurityStamp, opt => opt.Ignore())
            //.ForSourceMember(src => src.NewPassword, opt => opt.DoNotValidate());
        }
    }
}
