namespace FribergCarRental.Data
{
    public interface IRepository<T>
    {
        Task<T> AddAsync(T entity);
        void Delete(T entity);
        Task<T> GetByIdAsync(int? id);
        Task<IEnumerable<T>> GetAllAsync();
        T Update(T entity);
        Task SaveChangesAsync();
        Task<bool> ExistsAsync(int id);
    }
}
