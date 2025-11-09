using ShopManagement.Data.Context;
using ShopManagement.Interfaces;
using ShopManagement.Models.Entities;

namespace ShopManagement.Data.Repositories
{
    public class CategoryRepository : Repository<Category>, ICategoryRepository
    {
        public CategoryRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Category>> GetCategoriesByUserIdAsync(string userId)
        {
            return await _context.Categories
                .Include(c => c.SubCategories)
                .Where(c => c.CreatedBy == userId)
                .OrderBy(c => c.Name)
                .ToListAsync();
        }

        public async Task<IEnumerable<Category>> GetCategoriesWithProductsAsync(string userId)
        {
            return await _context.Categories
                .Include(c => c.Products.Where(p => p.IsActive))
                .Where(c => c.CreatedBy == userId)
                .ToListAsync();
        }

        public async Task<Category?> GetCategoryWithDetailsAsync(string id)
        {
            return await _context.Categories
                .Include(c => c.Products)
                .Include(c => c.SubCategories)
                .Include(c => c.ParentCategory)
                .FirstOrDefaultAsync(c => c.Id == id);
        }
    }
}
