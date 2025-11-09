namespace ShopManagement.Models.DTOs
{
    public class ProductDTOs
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Barcode { get; set; }
        public string CategoryId { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
        public decimal BuyingPrice { get; set; }
        public decimal SellingPrice { get; set; }
        public int CurrentStock { get; set; }
        public int MinStockLevel { get; set; }
        public string? SupplierId { get; set; }
        public string? SupplierName { get; set; }
        public bool IsActive { get; set; }
        public decimal ProfitPerUnit { get; set; }
        public decimal ProfitMargin { get; set; }
        public bool IsLowStock { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
