using FribergCarRental.Classes;

namespace FribergCarRental.Data
{
    public class ImageRepository : GenericRepository<Image>
    {
        public ImageRepository(ApplicationDbContext applicationDbContext) : base(applicationDbContext)
        {
        }

        public IEnumerable<Image> GetAllImagesByCarId(int id)
        {
            return applicationDbContext.Images.Where(i => i.Car.CarId == id).ToList();
        }
    }
}
