using ShopManagement.Models.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace ShopManagement.Interfaces
{
    public interface IProductHistoryRepository : IRepository<ProductHistory>
    {
        Task<IEnumerable<ProductHistory>> GetProductHistoryAsync(string productId);
        Task AddProductHistoryAsync(ProductHistory history);
    }
}
