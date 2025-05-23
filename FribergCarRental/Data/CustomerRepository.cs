using FribergCarRental.Classes;

namespace FribergCarRental.Data
{
    public class CustomerRepository : GenericRepository<Customer>
    {
        public CustomerRepository(ApplicationDbContext applicationDbContext) : base(applicationDbContext)
        {
        }
    }
}
