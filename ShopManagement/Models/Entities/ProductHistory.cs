using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShopManagement.Models.Entities
{
    public class ProductHistory
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [Required]
        public string ProductId { get; set; } = string.Empty;

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Required]
        [MaxLength(50)]
        public string TransactionType { get; set; } = string.Empty; // Purchase/Sale/Return/Adjustment

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

        [Required]
        public string CreatedBy { get; set; } = string.Empty;

        // Navigation properties
        [ForeignKey("ProductId")]
        public virtual Product Product { get; set; } = null!;

        [ForeignKey("CreatedBy")]
        public virtual User User { get; set; } = null!;
    }
}
