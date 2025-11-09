using ShopManagement.Models.Entities;

namespace ShopManagement.Interfaces
{
    public interface IProductHistoryRepository : IRepository<ProductHistory>
    {
        Task<IEnumerable<ProductHistory>> GetProductHistoryAsync(string productId);
        Task AddProductHistoryAsync(ProductHistory history);
    }
}
