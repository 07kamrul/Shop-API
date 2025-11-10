using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShopManagement.Models.Entities
{
    public class Sale
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [Required]
        public DateTime DateTime { get; set; } = DateTime.UtcNow;

        public string? CustomerId { get; set; }

        [MaxLength(255)]
        public string? CustomerName { get; set; }

        [MaxLength(20)]
        public string? CustomerPhone { get; set; }

        [Required]
        [MaxLength(50)]
        public string PaymentMethod { get; set; } = "cash";

        [Required]
        [Column(TypeName = "decimal(15,2)")]
        public decimal TotalAmount { get; set; }

        [Required]
        [Column(TypeName = "decimal(15,2)")]
        public decimal TotalCost { get; set; }

        [Required]
        [Column(TypeName = "decimal(15,2)")]
        public decimal TotalProfit { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Required]
        public string CreatedBy { get; set; } = string.Empty;

        // Navigation properties
        [ForeignKey("CustomerId")]
        public virtual Customer? Customer { get; set; }

        [ForeignKey("CreatedBy")]
        public virtual User User { get; set; } = null!;

        public virtual ICollection<SaleItem> SaleItems { get; set; } = new List<SaleItem>();
    }
}
