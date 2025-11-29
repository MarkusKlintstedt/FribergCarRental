namespace FribergCarRental.Models
{
    public class BookingViewModel //: IValidatableObject
    {
        public int BookingId { get; set; }
        public DateOnly RentStartDate { get; set; }
        public DateOnly RentEndDate { get; set; }
        public int CarId { get; set; }
        public CarViewModel? Car { get; set; }
        public UserViewModel? ApplicationUser { get; set; }
    }
}
