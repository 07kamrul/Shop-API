using ShopManagement.Models.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace ShopManagement.Interfaces
{
    public interface ISupplierRepository : IRepository<Supplier>
    {
        Task<IEnumerable<Supplier>> GetSuppliersByUserIdAsync(string userId);
        Task<IEnumerable<Supplier>> SearchSuppliersAsync(string userId, string searchTerm);
        Task<IEnumerable<Supplier>> GetTopSuppliersAsync(string userId, int limit = 10);
    }
}
