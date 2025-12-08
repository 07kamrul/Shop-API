// Services/InventoryService.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using ShopManagement.Interfaces;
using ShopManagement.Models.DTOs.Inventory;
using ShopManagement.Models.Entities;

namespace ShopManagement.Services;

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
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    public async Task<InventorySummaryDto> GetInventorySummaryAsync(string userId)
    {
        var products = await _unitOfWork.Products
            .FindAsync(p => p.CreatedBy == userId && p.IsActive);

        if (!products.Any())
            return new InventorySummaryDto(0, 0, 0, 0m, 0m);

        int lowStock = 0, outOfStock = 0;
        decimal stockValue = 0m, investment = 0m;

        foreach (var p in products)
        {
            var stock = p.CurrentStock;

            if (stock == 0) outOfStock++;
            else if (stock > 0 && stock <= p.MinStockLevel) lowStock++;

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
        Expression<Func<Product, object>> includeCategory = p => p.Category!;

        var products = await _unitOfWork.Products.FindAsync(
            p => p.CreatedBy == userId &&
                 p.IsActive &&
                 p.CurrentStock <= p.MinStockLevel,
            includeCategory
        );

        var alerts = products
            .Select(p => new StockAlertDto(
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
        Expression<Func<Product, object>> includeCategory = p => p.Category!;

        var products = await _unitOfWork.Products.FindAsync(
            p => p.CreatedBy == userId && p.IsActive,
            includeCategory
        );

        var categoryGroups = products
            .GroupBy(p => new
            {
                p.CategoryId,
                CategoryName = p.Category?.Name ?? "Uncategorized"
            })
            .Select(g => new CategoryInventoryDto(
                CategoryId: g.Key.CategoryId,
                CategoryName: g.Key.CategoryName,
                ProductCount: g.Count(),
                StockValue: g.Sum(p => p.CurrentStock * p.SellingPrice),
                LowStockCount: g.Count(p => p.CurrentStock <= p.MinStockLevel)
            ))
            .OrderByDescending(c => c.StockValue)
            .ToList();

        return categoryGroups;
    }

    public async Task<List<Product>> GetProductsNeedingRestockAsync(string userId)
    {
        var products = await _unitOfWork.Products.FindAsync(
            p => p.CreatedBy == userId &&
                 p.IsActive &&
                 p.CurrentStock <= p.MinStockLevel);

        return products
            .OrderBy(p => p.CurrentStock)
            .ThenBy(p => p.Name)
            .ToList();
    }

    public async Task<decimal> CalculateInventoryTurnoverAsync(string userId, DateTime startDate, DateTime endDate)
    {
        // Include SaleItems to calculate COGS properly
        Expression<Func<Sale, object>> includeSaleItems = s => s.SaleItems!;

        var sales = await _unitOfWork.Sales.FindAsync(
            s => s.CreatedBy == userId &&
                 s.DateTime >= startDate &&
                 s.DateTime <= endDate,
            includeSaleItems
        );

        // COGS = Σ(Quantity × BuyingPriceAtSale) — make sure SaleItem has BuyingPriceAtSale
        decimal cogs = sales
            .SelectMany(s => s.SaleItems)
            .Sum(item => item.Quantity * item.BuyingPriceAtSale); // ← critical field

        if (cogs <= 0m) return 0m;

        // Current inventory value (average would be better if you have historical data)
        var products = await _unitOfWork.Products
            .FindAsync(p => p.CreatedBy == userId && p.IsActive);

        decimal currentInventoryValue = products.Sum(p => p.CurrentStock * p.BuyingPrice);

        if (currentInventoryValue <= 0m) return 0m;

        decimal turnover = cogs / currentInventoryValue;

        return Math.Round(turnover, 2);
    }
}