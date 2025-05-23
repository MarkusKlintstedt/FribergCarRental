using FribergCarRental.Classes;

namespace FribergCarRental.Data
{
    public class BookingRepository : GenericRepository<Booking>
    {
        public BookingRepository(ApplicationDbContext applicationDbContext) : base(applicationDbContext)
        {
        }
    }
}
