using System.Linq.Expressions;

namespace ShopManagement.Interfaces
{
    public interface IGenericRepository<T> where T : class
    {
       Task<T?> GetByIdAsync(string id);
        Task<IEnumerable<T>> GetAllAsync();
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> expression);
        
        // New method with include support
        Task<IEnumerable<T>> FindAsync(
            Expression<Func<T, bool>> expression, 
            params Expression<Func<T, object>>[] includes);
        
        Task<T> AddAsync(T entity);
        Task AddRangeAsync(IEnumerable<T> entities);
        void Update(T entity);
        void Remove(T entity);
        void RemoveRange(IEnumerable<T> entities);
        Task<bool> ExistsAsync(Expression<Func<T, bool>> expression);
        Task<int> CountAsync(Expression<Func<T, bool>> expression = null);

    }
}
