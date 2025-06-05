namespace FribergCarRental.Data
{
    public abstract class GenericRepository<T> : IRepository<T> where T : class
    {
        private protected ApplicationDbContext applicationDbContext;
        public GenericRepository(ApplicationDbContext applicationDbContext)
        {
            this.applicationDbContext = applicationDbContext;
        }

        public virtual T Add(T entity)
        {
            var addedEntity = applicationDbContext.Add<T>(entity).Entity;
            return addedEntity;
        }

        public virtual void Delete(T entity)
        {
            applicationDbContext.Remove<T>(entity);
        }

        public virtual T GetById(int? id)
        {
            return applicationDbContext.Find<T>(id);
        }

        public virtual IEnumerable<T> GetAll()
        {
            var all = applicationDbContext.Set<T>().ToList();
            return all;
        }

        public virtual void SaveChanges()
        {
            applicationDbContext.SaveChanges();
        }

        public virtual T Update(T entity)
        {
            return applicationDbContext.Update<T>(entity).Entity;
        }
    }
}
