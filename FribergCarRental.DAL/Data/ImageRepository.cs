using FribergCarRental.Core.Classes;
using Microsoft.EntityFrameworkCore;

namespace FribergCarRental.DAL.Data
{
    public class ImageRepository : GenericRepository<Image>
    {
        public ImageRepository(ApplicationDbContext applicationDbContext) : base(applicationDbContext)
        {
        }

        public async Task<IEnumerable<Image>> GetAllImagesByCarIdAsync(int id)
        {
            return await applicationDbContext.Images.Where(i => i.CarId == id).ToListAsync();
        }
    }
}
