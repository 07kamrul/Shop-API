using System;
using System.ComponentModel.DataAnnotations;

namespace ShopManagement.Models.DTOs
{
    public class CustomerCreateRequest
    {
        [Required]
        [MaxLength(255)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(20)]
        public string? Phone { get; set; }

        [EmailAddress]
        public string? Email { get; set; }
        public string? Address { get; set; }
    }

    public class CustomerResponse
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public string? Address { get; set; }
        public decimal TotalPurchases { get; set; }
        public int TotalTransactions { get; set; }
        public DateTime LastPurchaseDate { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
