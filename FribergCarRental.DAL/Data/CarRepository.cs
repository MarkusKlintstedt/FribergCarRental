using FribergCarRental.DAL.Classes;

namespace FribergCarRental.DAL.Data
{
    public class CarRepository : GenericRepository<Car>
    {
        public CarRepository(ApplicationDbContext applicationDbContext) : base(applicationDbContext)
        {
        }

    }
}
