using Microsoft.EntityFrameworkCore;
using ShopManagement.API.Data.Context;
using ShopManagement.Interfaces;
using ShopManagement.Models.Entities;

namespace ShopManagement.Data.Repositories
{
    public class SaleRepository : Repository<Sale>, ISaleRepository
    {
        public SaleRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Sale>> GetSalesByUserIdAsync(string userId)
        {
            return await _context.Sales
                .Include(s => s.Customer)
                .Include(s => s.SaleItems)
                .Where(s => s.CreatedBy == userId)
                .OrderByDescending(s => s.DateTime)
                .ToListAsync();
        }

        public async Task<IEnumerable<Sale>> GetSalesByDateRangeAsync(string userId, DateTime startDate, DateTime endDate)
        {
            return await _context.Sales
                .Include(s => s.Customer)
                .Include(s => s.SaleItems)
                .Where(s => s.CreatedBy == userId &&
                           s.DateTime >= startDate &&
                           s.DateTime <= endDate)
                .OrderByDescending(s => s.DateTime)
                .ToListAsync();
        }

        public async Task<Sale?> GetSaleWithItemsAsync(string id)
        {
            return await _context.Sales
                .Include(s => s.Customer)
                .Include(s => s.SaleItems)
                    .ThenInclude(si => si.Product)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<IEnumerable<Sale>> GetTodaySalesAsync(string userId)
        {
            var today = DateTime.UtcNow.Date;
            var tomorrow = today.AddDays(1);

            return await _context.Sales
                .Include(s => s.Customer)
                .Include(s => s.SaleItems)
                .Where(s => s.CreatedBy == userId &&
                           s.DateTime >= today &&
                           s.DateTime < tomorrow)
                .OrderByDescending(s => s.DateTime)
                .ToListAsync();
        }
    }
}
