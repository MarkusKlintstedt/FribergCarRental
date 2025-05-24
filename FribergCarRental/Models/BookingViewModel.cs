using FribergCarRental.Classes;

namespace FribergCarRental.Models
{
    public class BookingViewModel
    {
        public int Id { get; set; }
        public DateOnly RentStartDate { get; set; }
        public DateOnly RentEndDate { get; set; }
        public required Car Car { get; set; }
        public required ApplicationUser ApplicationUser { get; set; }

    }
}
