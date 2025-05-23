using FribergCarRental.Classes;

namespace FribergCarRental.Data
{
    public class ImageRepository : GenericRepository<Image>
    {
        public ImageRepository(ApplicationDbContext applicationDbContext) : base(applicationDbContext)
        {
        }
    }
}
