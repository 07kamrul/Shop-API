using ShopManagement.API.Data.Context;
using ShopManagement.Interfaces;
using ShopManagement.Models.DTOs;
using ShopManagement.Models.Entities;

namespace ShopManagement.Services
{
    public interface ISaleService
    {
        Task<SaleResponse> CreateSaleAsync(SaleCreateRequest request, string userId);
        Task<SaleResponse> GetSaleByIdAsync(string id, string userId);
        Task<IEnumerable<SaleResponse>> GetSalesAsync(string userId, DateTime? startDate = null, DateTime? endDate = null);
        Task<bool> DeleteSaleAsync(string id, string userId);
        Task<IEnumerable<SaleResponse>> GetTodaySalesAsync(string userId);
    }

    public class SaleService : ISaleService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ApplicationDbContext _context;

        public SaleService(IUnitOfWork unitOfWork, ApplicationDbContext context)
        {
            _unitOfWork = unitOfWork;
            _context = context;
        }

        public async Task<SaleResponse> CreateSaleAsync(SaleCreateRequest request, string userId)
        {
            // Begin transaction properly
            await using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // Validate products and calculate totals
                decimal totalAmount = 0;
                decimal totalCost = 0;
                var saleItems = new List<SaleItem>();
                var productUpdates = new List<Product>();

                foreach (var itemRequest in request.Items)
                {
                    var product = await _unitOfWork.Products.GetByIdAsync(itemRequest.ProductId);
                    if (product == null || product.CreatedBy != userId)
                        throw new ArgumentException($"Product not found: {itemRequest.ProductId}");

                    if (product.CurrentStock < itemRequest.Quantity)
                        throw new InvalidOperationException($"Insufficient stock for product: {product.Name}");

                    // Calculate item details
                    var unitBuyingPrice = product.BuyingPrice;
                    var unitSellingPrice = itemRequest.UnitSellingPrice;
                    var itemTotalAmount = itemRequest.Quantity * unitSellingPrice;
                    var itemTotalCost = itemRequest.Quantity * unitBuyingPrice;
                    var itemTotalProfit = itemTotalAmount - itemTotalCost;

                    totalAmount += itemTotalAmount;
                    totalCost += itemTotalCost;

                    // Create sale item
                    var saleItem = new SaleItem
                    {
                        ProductId = product.Id,
                        ProductName = product.Name,
                        Quantity = itemRequest.Quantity,
                        UnitBuyingPrice = unitBuyingPrice,
                        UnitSellingPrice = unitSellingPrice,
                        TotalAmount = itemTotalAmount,
                        TotalCost = itemTotalCost,
                        TotalProfit = itemTotalProfit
                    };

                    saleItems.Add(saleItem);

                    // Update product stock
                    product.CurrentStock -= itemRequest.Quantity;
                    productUpdates.Add(product);

                    // Add product history
                    var productHistory = new ProductHistory
                    {
                        ProductId = product.Id,
                        TransactionType = "Sale",
                        QuantityChanged = -itemRequest.Quantity,
                        StockBefore = product.CurrentStock + itemRequest.Quantity,
                        StockAfter = product.CurrentStock,
                        UnitPrice = unitSellingPrice,
                        TotalValue = itemTotalAmount,
                        Notes = $"Sale of {itemRequest.Quantity} units",
                        CreatedBy = userId
                    };

                    await _unitOfWork.ProductHistories.AddAsync(productHistory);
                }

                // Update customer stats if customer exists
                Customer? customer = null;
                if (!string.IsNullOrEmpty(request.CustomerId))
                {
                    customer = await _unitOfWork.Customers.GetByIdAsync(request.CustomerId);
                    if (customer != null && customer.CreatedBy == userId)
                    {
                        customer.TotalPurchases += totalAmount;
                        customer.TotalTransactions += 1;
                        customer.LastPurchaseDate = DateTime.UtcNow;
                        _unitOfWork.Customers.Update(customer);
                    }
                }

                // Create sale
                var sale = new Sale
                {
                    DateTime = DateTime.UtcNow,
                    CustomerId = request.CustomerId,
                    CustomerName = request.CustomerName ?? customer?.Name,
                    CustomerPhone = request.CustomerPhone ?? customer?.Phone,
                    PaymentMethod = request.PaymentMethod,
                    TotalAmount = totalAmount,
                    TotalCost = totalCost,
                    TotalProfit = totalAmount - totalCost,
                    CreatedBy = userId,
                    CreatedAt = DateTime.UtcNow
                };

                await _unitOfWork.Sales.AddAsync(sale);

                // Update sale items with sale ID
                foreach (var saleItem in saleItems)
                {
                    saleItem.SaleId = sale.Id;
                    await _unitOfWork.SaleItems.AddAsync(saleItem);
                }

                // Update products
                foreach (var product in productUpdates)
                {
                    _unitOfWork.Products.Update(product);
                }

                // Commit all changes
                await _unitOfWork.CompleteAsync();
                await transaction.CommitAsync();

                // Return sale response
                return await GetSaleByIdAsync(sale.Id, userId);
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<SaleResponse> GetSaleByIdAsync(string id, string userId)
        {
            var sale = await _unitOfWork.Sales.GetByIdAsync(id);
            if (sale == null || sale.CreatedBy != userId)
                return null;

            var saleItems = (await _unitOfWork.SaleItems.FindAsync(si => si.SaleId == id))
                .Select(si => new SaleItemResponse
                {
                    Id = si.Id,
                    ProductId = si.ProductId,
                    ProductName = si.ProductName,
                    Quantity = si.Quantity,
                    UnitBuyingPrice = si.UnitBuyingPrice,
                    UnitSellingPrice = si.UnitSellingPrice,
                    TotalAmount = si.TotalAmount,
                    TotalCost = si.TotalCost,
                    TotalProfit = si.TotalProfit
                }).ToList();

            return new SaleResponse
            {
                Id = sale.Id,
                DateTime = sale.DateTime,
                CustomerId = sale.CustomerId,
                CustomerName = sale.CustomerName,
                CustomerPhone = sale.CustomerPhone,
                PaymentMethod = sale.PaymentMethod,
                TotalAmount = sale.TotalAmount,
                TotalCost = sale.TotalCost,
                TotalProfit = sale.TotalProfit,
                CreatedAt = sale.CreatedAt,
                Items = saleItems
            };
        }

        public async Task<IEnumerable<SaleResponse>> GetSalesAsync(string userId, DateTime? startDate = null, DateTime? endDate = null)
        {
            var sales = await _unitOfWork.Sales.FindAsync(s => s.CreatedBy == userId);

            if (startDate.HasValue)
                sales = sales.Where(s => s.DateTime >= startDate.Value).ToList();

            if (endDate.HasValue)
                sales = sales.Where(s => s.DateTime <= endDate.Value).ToList();

            var response = new List<SaleResponse>();
            foreach (var sale in sales.OrderByDescending(s => s.DateTime))
            {
                var saleItems = (await _unitOfWork.SaleItems.FindAsync(si => si.SaleId == sale.Id))
                    .Select(si => new SaleItemResponse
                    {
                        Id = si.Id,
                        ProductId = si.ProductId,
                        ProductName = si.ProductName,
                        Quantity = si.Quantity,
                        UnitBuyingPrice = si.UnitBuyingPrice,
                        UnitSellingPrice = si.UnitSellingPrice,
                        TotalAmount = si.TotalAmount,
                        TotalCost = si.TotalCost,
                        TotalProfit = si.TotalProfit
                    }).ToList();

                response.Add(new SaleResponse
                {
                    Id = sale.Id,
                    DateTime = sale.DateTime,
                    CustomerId = sale.CustomerId,
                    CustomerName = sale.CustomerName,
                    CustomerPhone = sale.CustomerPhone,
                    PaymentMethod = sale.PaymentMethod,
                    TotalAmount = sale.TotalAmount,
                    TotalCost = sale.TotalCost,
                    TotalProfit = sale.TotalProfit,
                    CreatedAt = sale.CreatedAt,
                    Items = saleItems
                });
            }

            return response;
        }

        public async Task<bool> DeleteSaleAsync(string id, string userId)
        {
            // Begin transaction
            await using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var sale = await _unitOfWork.Sales.GetByIdAsync(id);
                if (sale == null || sale.CreatedBy != userId)
                    return false;

                // Restore product stock
                var saleItems = await _unitOfWork.SaleItems.FindAsync(si => si.SaleId == id);
                foreach (var item in saleItems)
                {
                    var product = await _unitOfWork.Products.GetByIdAsync(item.ProductId);
                    if (product != null)
                    {
                        product.CurrentStock += item.Quantity;
                        _unitOfWork.Products.Update(product);

                        // Add product history for restoration
                        var productHistory = new ProductHistory
                        {
                            ProductId = product.Id,
                            TransactionType = "Sale Cancellation",
                            QuantityChanged = item.Quantity,
                            StockBefore = product.CurrentStock - item.Quantity,
                            StockAfter = product.CurrentStock,
                            UnitPrice = item.UnitSellingPrice,
                            TotalValue = item.TotalAmount,
                            Notes = $"Sale cancellation - restored {item.Quantity} units",
                            CreatedBy = userId
                        };

                        await _unitOfWork.ProductHistories.AddAsync(productHistory);
                    }
                }

                // Update customer stats if customer exists
                if (!string.IsNullOrEmpty(sale.CustomerId))
                {
                    var customer = await _unitOfWork.Customers.GetByIdAsync(sale.CustomerId);
                    if (customer != null)
                    {
                        customer.TotalPurchases -= sale.TotalAmount;
                        customer.TotalTransactions -= 1;
                        _unitOfWork.Customers.Update(customer);
                    }
                }

                // Remove sale items and sale
                _unitOfWork.SaleItems.RemoveRange(saleItems);
                _unitOfWork.Sales.Remove(sale);

                await _unitOfWork.CompleteAsync();
                await transaction.CommitAsync();

                return true;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<IEnumerable<SaleResponse>> GetTodaySalesAsync(string userId)
        {
            var today = DateTime.UtcNow.Date;
            var tomorrow = today.AddDays(1);

            return await GetSalesAsync(userId, today, tomorrow);
        }
    }
}
