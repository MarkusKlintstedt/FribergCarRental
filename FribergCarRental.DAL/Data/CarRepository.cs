using FribergCarRental.Classes;

namespace FribergCarRental.Data
{
    public class CarRepository : GenericRepository<Car>
    {
        public CarRepository(ApplicationDbContext applicationDbContext) : base(applicationDbContext)
        {
        }

    }
}
