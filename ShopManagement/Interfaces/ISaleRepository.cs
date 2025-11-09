using ShopManagement.Models.Entities;

namespace ShopManagement.Interfaces
{
    public interface ISaleRepository : IRepository<Sale>
    {
        Task<IEnumerable<Sale>> GetSalesByUserIdAsync(string userId);
        Task<IEnumerable<Sale>> GetSalesByDateRangeAsync(string userId, DateTime startDate, DateTime endDate);
        Task<Sale?> GetSaleWithItemsAsync(string id);
        Task<IEnumerable<Sale>> GetTodaySalesAsync(string userId);
    }
}
