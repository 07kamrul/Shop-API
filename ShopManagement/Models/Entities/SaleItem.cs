using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShopManagement.Models.Entities
{
    public class SaleItem : BaseEntity
    {
        [Required]
        public string SaleId { get; set; } = string.Empty;

        [Required]
        public string ProductId { get; set; } = string.Empty;

        [Required]
        [MaxLength(255)]
        public string ProductName { get; set; } = string.Empty;

        [Required]
        public int Quantity { get; set; }

        [Required]
        [Column(TypeName = "decimal(15,2)")]
        public decimal UnitBuyingPrice { get; set; }

        [Required]
        [Column(TypeName = "decimal(15,2)")]
        public decimal UnitSellingPrice { get; set; }

        [Required]
        [Column(TypeName = "decimal(15,2)")]
        public decimal TotalAmount { get; set; }

        [Required]
        [Column(TypeName = "decimal(15,2)")]
        public decimal TotalCost { get; set; }

        [Required]
        [Column(TypeName = "decimal(15,2)")]
        public decimal TotalProfit { get; set; }

        // Navigation properties
        [ForeignKey("SaleId")]
        public virtual Sale Sale { get; set; } = null!;

        [ForeignKey("ProductId")]
        public virtual Product Product { get; set; } = null!;
    }
}
