using Microsoft.EntityFrameworkCore;
using ShopManagement.API.Data.Context;
using ShopManagement.Interfaces;
using ShopManagement.Models.Entities;

namespace ShopManagement.Data.Repositories
{
    public class ProductHistoryRepository : Repository<ProductHistory>, IProductHistoryRepository
    {
        public ProductHistoryRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<ProductHistory>> GetProductHistoryAsync(string productId)
        {
            return await _context.ProductHistories
                .Where(ph => ph.ProductId == productId)
                .OrderByDescending(ph => ph.CreatedAt)
                .ToListAsync();
        }

        public async Task AddProductHistoryAsync(ProductHistory history)
        {
            await _context.ProductHistories.AddAsync(history);
        }
    }
}
