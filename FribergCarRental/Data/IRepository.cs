namespace FribergCarRental.Data
{
    public interface IRepository<T>
    {
        T Add(T entity);
        void Delete(T entity);
        T Get(int id);
        IEnumerable<T> GetAll();
        T Update(T entity);
        void SaveChanges();
    }
}
