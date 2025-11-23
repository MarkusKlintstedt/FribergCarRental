namespace FribergCarRental.Core.Dtos
{
    public class CreateBookingDto
    {
        public int CarId { get; set; }
        public string UserId { get; set; } = "";
        public DateOnly RentStartDate { get; set; }
        public DateOnly RentEndDate { get; set; }
    }
}
