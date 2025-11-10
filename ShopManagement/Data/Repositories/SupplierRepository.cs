using Microsoft.EntityFrameworkCore;
using ShopManagement.API.Data.Context;
using ShopManagement.Interfaces;
using ShopManagement.Models.Entities;

namespace ShopManagement.Data.Repositories
{
    public class SupplierRepository : Repository<Supplier>, ISupplierRepository
    {
        public SupplierRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Supplier>> GetSuppliersByUserIdAsync(string userId)
        {
            return await _context.Suppliers
                .Where(s => s.CreatedBy == userId)
                .OrderByDescending(s => s.LastPurchaseDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Supplier>> SearchSuppliersAsync(string userId, string searchTerm)
        {
            return await _context.Suppliers
                .Where(s => s.CreatedBy == userId &&
                           (s.Name.Contains(searchTerm) ||
                            s.ContactPerson != null && s.ContactPerson.Contains(searchTerm) ||
                            s.Phone != null && s.Phone.Contains(searchTerm) ||
                            s.Email != null && s.Email.Contains(searchTerm)))
                .ToListAsync();
        }

        public async Task<IEnumerable<Supplier>> GetTopSuppliersAsync(string userId, int limit = 10)
        {
            return await _context.Suppliers
                .Where(s => s.CreatedBy == userId)
                .OrderByDescending(s => s.TotalPurchases)
                .Take(limit)
                .ToListAsync();
        }
    }
}
