using System.ComponentModel.DataAnnotations;

namespace ShopManagement.Models.DTOs
{
    public class UpdateProductRequest
    {
        [Required]
        public string Name { get; set; } = string.Empty;
        public string? Barcode { get; set; }
        [Required]
        public string CategoryId { get; set; } = string.Empty;
        [Required]
        public decimal BuyingPrice { get; set; }
        [Required]
        public decimal SellingPrice { get; set; }
        public int MinStockLevel { get; set; } = 10;
        public string? SupplierId { get; set; }
    }
}
