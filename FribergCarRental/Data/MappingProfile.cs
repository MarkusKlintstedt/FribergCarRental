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
        }
    }
}
