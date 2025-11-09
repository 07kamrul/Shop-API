using System.ComponentModel.DataAnnotations;

namespace ShopManagement.Models.DTOs
{
    public class CreateProductRequest
    {
        [Required]
        public string Name { get; set; } = string.Empty;

        public string? Barcode { get; set; }

        [Required]
        public string CategoryId { get; set; } = string.Empty;

        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal BuyingPrice { get; set; }

        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal SellingPrice { get; set; }

        [Required]
        [Range(0, int.MaxValue)]
        public int CurrentStock { get; set; }

        [Range(0, int.MaxValue)]
        public int MinStockLevel { get; set; } = 10;

        public string? SupplierId { get; set; }
    }
}
