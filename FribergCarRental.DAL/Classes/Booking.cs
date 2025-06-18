using System.ComponentModel.DataAnnotations;

namespace FribergCarRental.DAL.Classes
{
    public class Booking
    {
        public int BookingId { get; set; }
        [Required]
        public DateOnly RentStartDate { get; set; }
        [Required]
        public DateOnly RentEndDate { get; set; }
        public int CarId { get; set; }
        public Car? Car { get; set; }
        public ApplicationUser? ApplicationUser { get; set; }
    }
}
