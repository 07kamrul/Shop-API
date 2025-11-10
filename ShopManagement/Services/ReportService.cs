using ShopManagement.Interfaces;
using ShopManagement.Models.DTOs;

namespace ShopManagement.Services
{
    public interface IReportService
    {
        Task<ProfitLossReport> GetProfitLossReportAsync(string userId, DateTime startDate, DateTime endDate);
        Task<IEnumerable<DailySalesReport>> GetDailySalesReportAsync(string userId, DateTime startDate, DateTime endDate);
        Task<IEnumerable<ProductSales>> GetTopSellingProductsAsync(string userId, DateTime startDate, DateTime endDate, int limit = 10);
    }

    public class ReportService : IReportService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ReportService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ProfitLossReport> GetProfitLossReportAsync(string userId, DateTime startDate, DateTime endDate)
        {
            var sales = await _unitOfWork.Sales.FindAsync(s =>
                s.CreatedBy == userId &&
                s.DateTime >= startDate &&
                s.DateTime <= endDate);

            var totalRevenue = sales.Sum(s => s.TotalAmount);
            var totalCost = sales.Sum(s => s.TotalCost);
            var grossProfit = totalRevenue - totalCost;
            var grossProfitMargin = totalRevenue > 0 ? (grossProfit / totalRevenue) * 100 : 0;

            // Get category breakdown
            var categoryBreakdown = new List<CategoryReport>();
            var categories = await _unitOfWork.Categories.FindAsync(c => c.CreatedBy == userId);

            foreach (var category in categories)
            {
                var categoryProducts = await _unitOfWork.Products.FindAsync(p =>
                    p.CategoryId == category.Id && p.CreatedBy == userId);

                var categoryProductIds = categoryProducts.Select(p => p.Id).ToList();
                var categorySaleItems = (await _unitOfWork.SaleItems.FindAsync(si =>
                    categoryProductIds.Contains(si.ProductId) &&
                    si.Sale.DateTime >= startDate &&
                    si.Sale.DateTime <= endDate &&
                    si.Sale.CreatedBy == userId)).ToList();

                var categorySales = categorySaleItems.Sum(si => si.TotalAmount);
                var categoryProfit = categorySaleItems.Sum(si => si.TotalProfit);
                var categoryMargin = categorySales > 0 ? (categoryProfit / categorySales) * 100 : 0;

                categoryBreakdown.Add(new CategoryReport
                {
                    CategoryId = category.Id,
                    CategoryName = category.Name,
                    TotalSales = categorySales,
                    TotalProfit = categoryProfit,
                    ProfitMargin = categoryMargin
                });
            }

            return new ProfitLossReport
            {
                StartDate = startDate,
                EndDate = endDate,
                TotalRevenue = totalRevenue,
                TotalCost = totalCost,
                GrossProfit = grossProfit,
                GrossProfitMargin = grossProfitMargin,
                CategoryBreakdown = categoryBreakdown
            };
        }

        public async Task<IEnumerable<DailySalesReport>> GetDailySalesReportAsync(string userId, DateTime startDate, DateTime endDate)
        {
            var sales = await _unitOfWork.Sales.FindAsync(s =>
                s.CreatedBy == userId &&
                s.DateTime >= startDate &&
                s.DateTime <= endDate);

            var dailyReports = sales
                .GroupBy(s => s.DateTime.Date)
                .Select(g => new DailySalesReport
                {
                    Date = g.Key,
                    TotalSales = g.Sum(s => s.TotalAmount),
                    TotalProfit = g.Sum(s => s.TotalProfit),
                    TotalTransactions = g.Count()
                })
                .OrderByDescending(r => r.Date)
                .ToList();

            // Get top products for each day
            foreach (var report in dailyReports)
            {
                var daySales = sales.Where(s => s.DateTime.Date == report.Date).ToList();
                var saleIds = daySales.Select(s => s.Id).ToList();
                var saleItems = await _unitOfWork.SaleItems.FindAsync(si => saleIds.Contains(si.SaleId));

                var topProducts = saleItems
                    .GroupBy(si => new { si.ProductId, si.ProductName })
                    .Select(g => new ProductSales
                    {
                        ProductId = g.Key.ProductId,
                        ProductName = g.Key.ProductName,
                        QuantitySold = g.Sum(si => si.Quantity),
                        TotalSales = g.Sum(si => si.TotalAmount),
                        TotalProfit = g.Sum(si => si.TotalProfit)
                    })
                    .OrderByDescending(p => p.TotalSales)
                    .Take(5)
                    .ToList();

                report.TopProducts = topProducts;
            }

            return dailyReports;
        }

        public async Task<IEnumerable<ProductSales>> GetTopSellingProductsAsync(string userId, DateTime startDate, DateTime endDate, int limit = 10)
        {
            var sales = await _unitOfWork.Sales.FindAsync(s =>
                s.CreatedBy == userId &&
                s.DateTime >= startDate &&
                s.DateTime <= endDate);

            var saleIds = sales.Select(s => s.Id).ToList();
            var saleItems = await _unitOfWork.SaleItems.FindAsync(si => saleIds.Contains(si.SaleId));

            var topProducts = saleItems
                .GroupBy(si => new { si.ProductId, si.ProductName })
                .Select(g => new ProductSales
                {
                    ProductId = g.Key.ProductId,
                    ProductName = g.Key.ProductName,
                    QuantitySold = g.Sum(si => si.Quantity),
                    TotalSales = g.Sum(si => si.TotalAmount),
                    TotalProfit = g.Sum(si => si.TotalProfit)
                })
                .OrderByDescending(p => p.TotalSales)
                .Take(limit)
                .ToList();

            return topProducts;
        }
    }
}
