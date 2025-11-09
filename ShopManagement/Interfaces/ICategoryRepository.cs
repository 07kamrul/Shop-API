using ShopManagement.Models.Entities;

namespace ShopManagement.Interfaces
{
    public interface ICategoryRepository : IRepository<Category>
    {
        Task<IEnumerable<Category>> GetCategoriesByUserIdAsync(string userId);
        Task<IEnumerable<Category>> GetCategoriesWithProductsAsync(string userId);
        Task<Category?> GetCategoryWithDetailsAsync(string id);
    }
}
