// Models/DTOs/Inventory/InventorySummaryDto.cs
namespace ShopManagement.Models.DTOs.Inventory
{
    public record InventorySummaryDto(
        int TotalProducts,
        int LowStockItems,
        int OutOfStockItems,
        decimal TotalStockValue,
        decimal TotalInvestment
    );
}

// Models/DTOs/Inventory/StockAlertDto.cs
namespace ShopManagement.Models.DTOs.Inventory
{
    public record StockAlertDto(
        string ProductId,
        string ProductName,
        string CategoryName,
        int CurrentStock,
        int MinStockLevel,
        string AlertType // "low_stock" or "out_of_stock"
    );
}

// Models/DTOs/Inventory/CategoryInventoryDto.cs
namespace ShopManagement.Models.DTOs.Inventory
{
    public record CategoryInventoryDto(
        string CategoryId,
        string CategoryName,
        int ProductCount,
        decimal StockValue,
        int LowStockCount
    );
}