using FribergCarRental.Core.Classes;
using Microsoft.EntityFrameworkCore;

namespace FribergCarRental.DAL.Data
{
    public class CarRepository : GenericRepository<Car>
    {
        public CarRepository(ApplicationDbContext applicationDbContext) : base(applicationDbContext)
        {

        }

        public async Task<List<Car>> GetCarsWithImagesAsync()
        {
            return await applicationDbContext.Cars.Include(c => c.Images).ToListAsync();
        }

        public async Task<Car> GetCarWithImagesAsync(int id)
        {
            return await applicationDbContext.Cars.Include(c => c.Images).FirstOrDefaultAsync(c => c.CarId == id);
        }

    }
}
