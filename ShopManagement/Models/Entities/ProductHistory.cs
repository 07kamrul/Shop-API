using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShopManagement.Models.Entities
{
    public class ProductHistory : BaseEntity
    {
        [Required]
        public string ProductId { get; set; } = string.Empty;

        [Required]
        [MaxLength(20)]
        public string TransactionType { get; set; } = string.Empty; // Purchase, Sale, Return, Adjustment

        [Required]
        public int QuantityChanged { get; set; }

        [Required]
        public int StockBefore { get; set; }

        [Required]
        public int StockAfter { get; set; }

        [Column(TypeName = "decimal(15,2)")]
        public decimal? UnitPrice { get; set; }

        [Column(TypeName = "decimal(15,2)")]
        public decimal? TotalValue { get; set; }

        public string? Notes { get; set; }

        // Navigation properties
        [ForeignKey("ProductId")]
        public virtual Product Product { get; set; } = null!;
    }
}
