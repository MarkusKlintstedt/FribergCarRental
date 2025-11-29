using FribergCarRental.Core.Classes;

namespace FribergCarRental.DAL.Data
{
    public class ImageRepository : GenericRepository<Image>
    {
        public ImageRepository(ApplicationDbContext applicationDbContext) : base(applicationDbContext)
        {
        }

        //public async Task<IEnumerable<Image>> GetAllImagesByCarIdAsync(int id)
        //{
        //    return await applicationDbContext.Images.Where(i => i.CarId == id).ToListAsync();
        //}
    }
}
