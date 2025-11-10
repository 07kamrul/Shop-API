using Microsoft.EntityFrameworkCore;
using ShopManagement.API.Data.Context;
using ShopManagement.Interfaces;
using ShopManagement.Models.Entities;

namespace ShopManagement.Data.Repositories
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        public ProductRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Product>> GetProductsByUserIdAsync(string userId)
        {
            return await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Supplier)
                .Where(p => p.CreatedBy == userId && p.IsActive)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetLowStockProductsAsync(string userId)
        {
            return await _context.Products
                .Include(p => p.Category)
                .Where(p => p.CreatedBy == userId &&
                           p.IsActive &&
                           p.CurrentStock <= p.MinStockLevel)
                .ToListAsync();
        }

        public async Task<Product?> GetProductWithDetailsAsync(string id)
        {
            return await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Supplier)
                .Include(p => p.SaleItems)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task UpdateStockAsync(string productId, int newStock)
        {
            var product = await GetByIdAsync(productId);
            if (product != null)
            {
                product.CurrentStock = newStock;
                Update(product);
            }
        }

        public async Task<IEnumerable<Product>> SearchProductsAsync(string userId, string searchTerm)
        {
            return await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Supplier)
                .Where(p => p.CreatedBy == userId &&
                           p.IsActive &&
                           (p.Name.Contains(searchTerm) ||
                            p.Barcode != null && p.Barcode.Contains(searchTerm)))
                .ToListAsync();
        }
    }
}
