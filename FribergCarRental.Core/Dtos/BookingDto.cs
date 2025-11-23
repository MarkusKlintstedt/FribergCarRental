using System.ComponentModel.DataAnnotations;

namespace FribergCarRental.Core.Dtos
{
    public class BookingDto
    {
        public int BookingId { get; set; }
        [Required]
        public DateOnly RentStartDate { get; set; }
        [Required]
        public DateOnly RentEndDate { get; set; }
        public int CarId { get; set; }
        public CarDto Car { get; set; }
        public ApplicationUserDto ApplicationUser { get; set; }
    }
}
