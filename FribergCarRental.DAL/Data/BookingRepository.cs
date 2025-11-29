using FribergCarRental.Core.Classes;
using Microsoft.EntityFrameworkCore;

namespace FribergCarRental.DAL.Data
{
    public class BookingRepository : GenericRepository<Booking>
    {
        public BookingRepository(ApplicationDbContext applicationDbContext) : base(applicationDbContext)
        {
        }

        public override async Task<IEnumerable<Booking>> GetAllAsync()
        {
            return await applicationDbContext.Bookings
                .Include(b => b.Car)
                .Include(b => b.ApplicationUser)
                .ToListAsync();
        }

        public override async Task<Booking?> GetByIdAsync(int? id)
        {
            return await applicationDbContext.Bookings
                .Include(b => b.Car)
                .Include(b => b.ApplicationUser)
                .FirstOrDefaultAsync(b => b.BookingId == id);
        }

        public async Task<List<Booking>> GetAllByCarIdAsync(int carId)
        {
            return await applicationDbContext.Bookings
                .Where(b => b.Car != null && b.Car.CarId == carId)
                .ToListAsync();
        }
        public async Task<IEnumerable<Booking>> GetAllByUserIdAsync(string userId)
        {
            return await applicationDbContext.Bookings
                .Include(b => b.Car)
                .Include(b => b.ApplicationUser)
                .Where(b => b.ApplicationUser != null && b.ApplicationUser.Id == userId)
                .ToListAsync();
        }
    }
}
