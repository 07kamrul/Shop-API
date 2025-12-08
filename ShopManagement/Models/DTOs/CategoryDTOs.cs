using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ShopManagement.Models.DTOs
{
    public class CategoryCreateRequest
    {
        [Required]
        [MaxLength(255)]
        public string Name { get; set; } = string.Empty;

        public string? ParentCategoryId { get; set; }
        public string? Description { get; set; }

        [Range(0, 100)]
        public decimal? ProfitMarginTarget { get; set; }
    }

    public class CategoryResponse
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? ParentCategoryId { get; set; }
        public string? ParentCategoryName { get; set; }
        public string? Description { get; set; }
        public decimal? ProfitMarginTarget { get; set; }
        public DateTime CreatedAt { get; set; }
        public int ProductCount { get; set; }
        public IEnumerable<CategoryResponse> SubCategories { get; set; } = new List<CategoryResponse>();
    }
}
