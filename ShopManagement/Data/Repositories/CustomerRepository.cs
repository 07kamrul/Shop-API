using Microsoft.EntityFrameworkCore;
using ShopManagement.API.Data.Context;
using ShopManagement.Interfaces;
using ShopManagement.Models.Entities;

namespace ShopManagement.Data.Repositories
{
    public class CustomerRepository : Repository<Customer>, ICustomerRepository
    {
        public CustomerRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Customer>> GetCustomersByUserIdAsync(string userId)
        {
            return await _context.Customers
                .Where(c => c.CreatedBy == userId)
                .OrderByDescending(c => c.LastPurchaseDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Customer>> SearchCustomersAsync(string userId, string searchTerm)
        {
            return await _context.Customers
                .Where(c => c.CreatedBy == userId &&
                           (c.Name.Contains(searchTerm) ||
                            c.Phone != null && c.Phone.Contains(searchTerm) ||
                            c.Email != null && c.Email.Contains(searchTerm)))
                .ToListAsync();
        }

        public async Task<IEnumerable<Customer>> GetTopCustomersAsync(string userId, int limit = 10)
        {
            return await _context.Customers
                .Where(c => c.CreatedBy == userId)
                .OrderByDescending(c => c.TotalPurchases)
                .Take(limit)
                .ToListAsync();
        }

        public async Task UpdateCustomerPurchaseStatsAsync(string customerId, decimal purchaseAmount)
        {
            var customer = await GetByIdAsync(customerId);
            if (customer != null)
            {
                customer.TotalPurchases += purchaseAmount;
                customer.TotalTransactions += 1;
                customer.LastPurchaseDate = DateTime.UtcNow;
                Update(customer);
            }
        }
    }
}
