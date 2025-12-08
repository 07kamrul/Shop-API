using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShopManagement.Models.Entities
{
    public class Product
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [Required]
        [MaxLength(255)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(255)]
        public string? Barcode { get; set; }

        [Required]
        public string CategoryId { get; set; } = string.Empty;

        [Required]
        [Column(TypeName = "decimal(15,2)")]
        public decimal BuyingPrice { get; set; }

        [Required]
        [Column(TypeName = "decimal(15,2)")]
        public decimal SellingPrice { get; set; }

        [Required]
        public int CurrentStock { get; set; }

        public int MinStockLevel { get; set; } = 10;

        public string? SupplierId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsActive { get; set; } = true;

        [Required]
        public string CreatedBy { get; set; } = string.Empty;

        // Navigation properties
        [ForeignKey("CategoryId")]
        public virtual Category Category { get; set; } = null!;

        [ForeignKey("SupplierId")]
        public virtual Supplier? Supplier { get; set; }

        [ForeignKey("CreatedBy")]
        public virtual User User { get; set; } = null!;

        public virtual ICollection<SaleItem> SaleItems { get; set; } = new List<SaleItem>();
        public virtual ICollection<ProductHistory> ProductHistories { get; set; } = new List<ProductHistory>();

        // Calculated properties
        [NotMapped]
        public decimal ProfitPerUnit => SellingPrice - BuyingPrice;

        [NotMapped]
        public decimal ProfitMargin => SellingPrice > 0 ? (ProfitPerUnit / SellingPrice) * 100 : 0;

        [NotMapped]
        public bool IsLowStock => CurrentStock <= MinStockLevel;
    }
}
