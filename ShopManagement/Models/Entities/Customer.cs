using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShopManagement.Models.Entities
{
    public class Customer
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [Required]
        [MaxLength(255)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(20)]
        public string? Phone { get; set; }

        [EmailAddress]
        [MaxLength(255)]
        public string? Email { get; set; }

        public string? Address { get; set; }

        [Column(TypeName = "decimal(15,2)")]
        public decimal TotalPurchases { get; set; } = 0;

        public int TotalTransactions { get; set; } = 0;

        public DateTime LastPurchaseDate { get; set; } = DateTime.UtcNow;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Required]
        public string CreatedBy { get; set; } = string.Empty;

        // Navigation properties
        [ForeignKey("CreatedBy")]
        public virtual User User { get; set; } = null!;
        public virtual ICollection<Sale> Sales { get; set; } = new List<Sale>();
    }
}
