using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShopManagement.Models.Entities
{
    public class Category
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        public string? ParentCategoryId { get; set; }
        public string? Description { get; set; }

        [Column(TypeName = "decimal(5,2)")]
        public decimal? ProfitMarginTarget { get; set; }

        [Required]
        public string CreatedBy { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        [ForeignKey("ParentCategoryId")]
        public virtual Category? ParentCategory { get; set; }

        [ForeignKey("CreatedBy")]
        public virtual User User { get; set; } = null!;

        public virtual ICollection<Product> Products { get; set; } = new List<Product>();
        public virtual ICollection<Category> SubCategories { get; set; } = new List<Category>();
    }
}
