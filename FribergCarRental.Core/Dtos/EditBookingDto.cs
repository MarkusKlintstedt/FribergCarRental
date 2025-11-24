namespace FribergCarRental.Core.Dtos
{
    public class EditBookingDto
    {
        public int BookingId { get; set; }
        public int CarId { get; set; }
        public string UserId { get; set; } = "";
        public DateOnly RentStartDate { get; set; }
        public DateOnly RentEndDate { get; set; }

    }
}
