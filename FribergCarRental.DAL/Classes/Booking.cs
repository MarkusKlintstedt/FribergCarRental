using System.ComponentModel.DataAnnotations;

namespace FribergCarRental.Classes
{
    public class Booking
    {
        public int BookingId { get; set; }
        [Required]
        public DateOnly RentStartDate { get; set; }
        [Required]
        public DateOnly RentEndDate { get; set; }
        public required Car Car { get; set; }
        public required ApplicationUser ApplicationUser { get; set; }
    }
}
