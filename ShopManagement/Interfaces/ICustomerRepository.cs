using ShopManagement.Models.Entities;

namespace ShopManagement.Interfaces
{
    public interface ICustomerRepository : IRepository<Customer>
    {
        Task<IEnumerable<Customer>> GetCustomersByUserIdAsync(string userId);
        Task<IEnumerable<Customer>> SearchCustomersAsync(string userId, string searchTerm);
        Task<IEnumerable<Customer>> GetTopCustomersAsync(string userId, int limit = 10);
        Task UpdateCustomerPurchaseStatsAsync(string customerId, decimal purchaseAmount);
    }
}
