using Microsoft.EntityFrameworkCore;

namespace FribergCarRental.Data
{
    public abstract class GenericRepository<T> : IRepository<T> where T : class
    {
        private protected ApplicationDbContext applicationDbContext;
        public GenericRepository(ApplicationDbContext applicationDbContext)
        {
            this.applicationDbContext = applicationDbContext;
        }

        public virtual async Task<T> AddAsync(T entity)
        {
            var addedEntity = await applicationDbContext.AddAsync<T>(entity);
            return addedEntity.Entity;
        }

        public void Delete(T entity)
        {
            applicationDbContext.Remove<T>(entity);
        }

        public async Task<T> GetByIdAsync(int? id)
        {
            return await applicationDbContext.FindAsync<T>(id);
        }

        public virtual async Task<IEnumerable<T>> GetAllAsync()
        {
            var all = await applicationDbContext.Set<T>().ToListAsync();
            return all;
        }

        public virtual Task SaveChangesAsync()
        {
            return applicationDbContext.SaveChangesAsync();
        }

        public virtual T Update(T entity)
        {
            return applicationDbContext.Update<T>(entity).Entity;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            var entity = await applicationDbContext.Set<T>().FindAsync(id);
            return entity != null;
        }
    }
}
