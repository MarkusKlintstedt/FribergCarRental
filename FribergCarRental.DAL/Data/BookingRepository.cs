using FribergCarRental.DAL.Classes;
using Microsoft.EntityFrameworkCore;

namespace FribergCarRental.DAL.Data
{
    public class BookingRepository : GenericRepository<Booking>
    {
        public BookingRepository(ApplicationDbContext applicationDbContext) : base(applicationDbContext)
        {
        }
        public async Task<IEnumerable<Booking>> GetAllWithCarAsync()
        {
            return await applicationDbContext.Bookings.Include(b => b.Car).ToListAsync();
        }

        public async Task<IEnumerable<Booking>> GetAllWithCarAndUserAsync()
        {
            return await applicationDbContext.Bookings
                .Include(b => b.Car)
                .Include(b => b.ApplicationUser)
                .ToListAsync();
        }
        public async Task<IEnumerable<Booking>> GetAllBookingsWithCarByUserIdAsync(string userId)
        {
            return await applicationDbContext.Bookings
                .Include(b => b.Car)
                .Include(b => b.ApplicationUser)
                .Where(b => b.ApplicationUser != null && b.ApplicationUser.Id == userId)
                .ToListAsync();
        }

        public async Task<Booking?> GetBookingWithCarByIdAsync(int? id)
        {
            return await applicationDbContext.Bookings
                .Include(b => b.Car)
                .FirstOrDefaultAsync(b => b.BookingId == id);
        }

        public async Task<List<Booking>> GetAllBookingsByCarIdAsync(int carId)
        {
            return await applicationDbContext.Bookings
                .Where(b => b.Car != null && b.Car.CarId == carId)
                .ToListAsync();
        }
    }
}
