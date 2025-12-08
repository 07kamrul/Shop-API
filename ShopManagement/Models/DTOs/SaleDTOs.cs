using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ShopManagement.Models.DTOs
{
    public class SaleItemRequest
    {
        [Required]
        public string ProductId { get; set; } = string.Empty;

        [Required]
        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }

        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal UnitSellingPrice { get; set; }
    }

    public class SaleCreateRequest
    {
        public string? CustomerId { get; set; }
        public string? CustomerName { get; set; }
        public string? CustomerPhone { get; set; }

        [Required]
        public string PaymentMethod { get; set; } = "cash";

        [Required]
        public List<SaleItemRequest> Items { get; set; } = new();
    }

    public class SaleItemResponse
    {
        public string Id { get; set; } = string.Empty;
        public string ProductId { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal UnitBuyingPrice { get; set; }
        public decimal UnitSellingPrice { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal TotalCost { get; set; }
        public decimal TotalProfit { get; set; }
    }

    public class SaleResponse
    {
        public string Id { get; set; } = string.Empty;
        public DateTime DateTime { get; set; }
        public string? CustomerId { get; set; }
        public string? CustomerName { get; set; }
        public string? CustomerPhone { get; set; }
        public string PaymentMethod { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public decimal TotalCost { get; set; }
        public decimal TotalProfit { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<SaleItemResponse> Items { get; set; } = new();
    }
}
