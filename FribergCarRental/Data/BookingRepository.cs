using FribergCarRental.Classes;
using Microsoft.EntityFrameworkCore;

namespace FribergCarRental.Data
{
    public class BookingRepository : GenericRepository<Booking>
    {
        public BookingRepository(ApplicationDbContext applicationDbContext) : base(applicationDbContext)
        {
        }
        public IEnumerable<Booking> GetAllWithCar()
        {
            return applicationDbContext.Bookings.Include(b => b.Car).ToList();
        }

        public IEnumerable<Booking> GetAllWithCarAndUser()
        {
            return applicationDbContext.Bookings
                .Include(b => b.Car)
                .Include(b => b.ApplicationUser)
                .ToList();
        }
        public IEnumerable<Booking> GetAllWithCarAndUserByUserId(string userId)
        {
            return applicationDbContext.Bookings
                .Include(b => b.Car)
                //.Include(b => b.ApplicationUser)
                .Where(b => b.ApplicationUser.Id == userId)
                .ToList();
        }
    }
}
