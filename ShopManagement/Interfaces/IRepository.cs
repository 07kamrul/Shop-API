using System.Linq.Expressions;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
namespace ShopManagement.Interfaces
{
    public interface IRepository<T> where T : class
    {
        Task<T?> GetByIdAsync(string id);
        Task<IEnumerable<T>> GetAllAsync();
        Task AddAsync(T entity);
        void Update(T entity);
        void Delete(T entity);
        Task<bool> ExistsAsync(string id);
    
        Task<IEnumerable<T>> GetAllWithIncludesAsync(
        Expression<Func<T, bool>> filter = null,
        params Expression<Func<T, object>>[] includes);
    }
}
