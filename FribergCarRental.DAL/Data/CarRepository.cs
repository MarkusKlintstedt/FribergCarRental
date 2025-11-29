using FribergCarRental.Core.Classes;
using Microsoft.EntityFrameworkCore;

namespace FribergCarRental.DAL.Data
{
    public class CarRepository : GenericRepository<Car>
    {
        public CarRepository(ApplicationDbContext applicationDbContext) : base(applicationDbContext)
        {

        }

        public override async Task<IEnumerable<Car>> GetAllAsync()
        {
            return await applicationDbContext.Cars.Include(c => c.Images).ToListAsync();
        }

        public override async Task<Car> GetByIdAsync(int? id)
        {
            return await applicationDbContext.Cars.Include(c => c.Images).FirstOrDefaultAsync(c => c.CarId == id);
        }
    }
}
