// Services/IInventoryService.cs
namespace ShopManagement.Services
{
    public interface IInventoryService
    {
        Task<InventorySummaryDto> GetInventorySummaryAsync(string userId);
        Task<List<StockAlertDto>> GetStockAlertsAsync(string userId);
        Task<List<CategoryInventoryDto>> GetCategoryInventoryAsync(string userId);
        Task<List<Product>> GetProductsNeedingRestockAsync(string userId);
        Task<decimal> CalculateInventoryTurnoverAsync(string userId, DateTime startDate, DateTime endDate);
    }
    public class InventoryService : IInventoryService
    {
        private readonly IUnitOfWork _unitOfWork;

        public InventoryService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<InventorySummaryDto> GetInventorySummaryAsync(string userId)
        {
            var products = await _unitOfWork.Products
                .FindAsync(p => p.CreatedBy == userId && p.IsActive);

            if (!products.Any())
            {
                return new InventorySummaryDto(0, 0, 0, 0m, 0m);
            }

            int lowStock = 0, outOfStock = 0;
            decimal stockValue = 0m, investment = 0m;

            foreach (var p in products)
            {
                var stock = p.CurrentStock;

                if (stock == 0) outOfStock++;
                else if (stock <= p.MinStockLevel) lowStock++;

                stockValue += stock * p.SellingPrice;
                investment += stock * p.BuyingPrice;
            }

            return new InventorySummaryDto(
                TotalProducts: products.Count(),
                LowStockItems: lowStock,
                OutOfStockItems: outOfStock,
                TotalStockValue: stockValue,
                TotalInvestment: investment
            );
        }

        public async Task<List<StockAlertDto>> GetStockAlertsAsync(string userId)
        {
            var products = await _unitOfWork.Products
                .FindAsync(
                    p => p.CreatedBy == userId && p.IsActive && (p.CurrentStock <= p.MinStockLevel),
                    include: p => p.Category
                );

            var alerts = products.Select(p => new StockAlertDto(
                ProductId: p.Id,
                ProductName: p.Name,
                CategoryName: p.Category?.Name ?? "Unknown",
                CurrentStock: p.CurrentStock,
                MinStockLevel: p.MinStockLevel,
                AlertType: p.CurrentStock == 0 ? "out_of_stock" : "low_stock"
            ))
            .OrderByDescending(a => a.AlertType == "out_of_stock")
            .ThenBy(a => a.CurrentStock)
            .ToList();

            return alerts;
        }

        public async Task<List<CategoryInventoryDto>> GetCategoryInventoryAsync(string userId)
        {
            var categories = await _unitOfWork.Categories
                .FindAsync(c => c.CreatedBy == userId);

            var products = await _unitOfWork.Products
                .FindAsync(p => p.CreatedBy == userId && p.IsActive, include: p => p.Category);

            var categoryMap = categories.ToDictionary(
                c => c.Id,
                c => new CategoryInventoryDto(c.Id, c.Name, 0, 0m, 0)
            );

            foreach (var p in products)
            {
                if (!categoryMap.TryGetValue(p.CategoryId, out var cat)) continue;

                var updated = cat with
                {
                    ProductCount = cat.ProductCount + 1,
                    StockValue = cat.StockValue + (p.CurrentStock * p.SellingPrice),
                    LowStockCount = cat.LowStockCount + (p.CurrentStock <= p.MinStockLevel ? 1 : 0)
                };
                categoryMap[p.CategoryId] = updated;
            }

            return categoryMap.Values
                .OrderByDescending(c => c.StockValue)
                .ToList();
        }

        public async Task<List<Product>> GetProductsNeedingRestockAsync(string userId)
        {
            var products = await _unitOfWork.Products
                .FindAsync(p => p.CreatedBy == userId && p.IsActive && p.CurrentStock <= p.MinStockLevel);

            return products.OrderBy(p => p.CurrentStock).ToList();
        }

        public async Task<decimal> CalculateInventoryTurnoverAsync(string userId, DateTime startDate, DateTime endDate)
        {
            var sales = await _unitOfWork.Sales
                .FindAsync(s => s.CreatedBy == userId && s.DateTime >= startDate && s.DateTime <= endDate);

            var products = await _unitOfWork.Products
                .FindAsync(p => p.CreatedBy == userId && p.IsActive);

            if (!sales.Any() || !products.Any()) return 0m;

            var totalSalesAmount = sales.Sum(s => s.TotalAmount);
            if (totalSalesAmount == 0) return 0m;

            var totalInventoryValue = products.Sum(p => p.CurrentStock * p.BuyingPrice);
            var avgInventoryValue = products.Any() ? totalInventoryValue / products.Count() : 0m;

            return avgInventoryValue > 0 ? totalSalesAmount / avgInventoryValue : 0m;
        }
    }
}