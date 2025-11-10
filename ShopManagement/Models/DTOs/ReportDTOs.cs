using System.ComponentModel.DataAnnotations;

namespace ShopManagement.Models.DTOs
{
    public class DateRangeRequest
    {
        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }
    }

    public class DailySalesReport
    {
        public DateTime Date { get; set; }
        public decimal TotalSales { get; set; }
        public decimal TotalProfit { get; set; }
        public int TotalTransactions { get; set; }
        public List<ProductSales> TopProducts { get; set; } = new();
    }

    public class ProductSales
    {
        public string ProductId { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public int QuantitySold { get; set; }
        public decimal TotalSales { get; set; }
        public decimal TotalProfit { get; set; }
    }

    public class ProfitLossReport
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal TotalCost { get; set; }
        public decimal GrossProfit { get; set; }
        public decimal GrossProfitMargin { get; set; }
        public List<CategoryReport> CategoryBreakdown { get; set; } = new();
    }

    public class CategoryReport
    {
        public string CategoryId { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
        public decimal TotalSales { get; set; }
        public decimal TotalProfit { get; set; }
        public decimal ProfitMargin { get; set; }
    }
}
