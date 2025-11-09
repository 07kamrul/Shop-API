using ShopManagement.Models.Entities;

namespace ShopManagement.Interfaces
{
    public interface IProductRepository : IRepository<Product>
    {
        Task<IEnumerable<Product>> GetProductsByUserIdAsync(string userId);
        Task<IEnumerable<Product>> GetLowStockProductsAsync(string userId);
        Task<Product?> GetProductWithDetailsAsync(string id);
        Task UpdateStockAsync(string productId, int newStock);
        Task<IEnumerable<Product>> SearchProductsAsync(string userId, string searchTerm);
    }
}
