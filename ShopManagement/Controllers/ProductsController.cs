using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopManagement.Interfaces;
using ShopManagement.Models.DTOs;
using ShopManagement.Models.Entities;
using System.Security.Claims;

namespace ShopManagement.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public ProductsController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductResponse>>> GetProducts()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var products = await _unitOfWork.Products.FindAsync(p => p.CreatedBy == userId && p.IsActive);

            // Manually load Category and Supplier for each product
            var response = new List<ProductResponse>();
            foreach (var p in products)
            {
                var category = p.Category ?? await _unitOfWork.Categories.GetByIdAsync(p.CategoryId);
                var supplier = p.Supplier;
                if (supplier == null && p.SupplierId != null)
                {
                    supplier = await _unitOfWork.Suppliers.GetByIdAsync(p.SupplierId);
                }

                response.Add(new ProductResponse
                {
                    Id = p.Id,
                    Name = p.Name,
                    Barcode = p.Barcode,
                    CategoryId = p.CategoryId,
                    CategoryName = category?.Name ?? "Unknown",
                    BuyingPrice = p.BuyingPrice,
                    SellingPrice = p.SellingPrice,
                    CurrentStock = p.CurrentStock,
                    MinStockLevel = p.MinStockLevel,
                    SupplierId = p.SupplierId,
                    SupplierName = supplier?.Name,
                    CreatedAt = p.CreatedAt,
                    IsActive = p.IsActive,
                    ProfitPerUnit = p.ProfitPerUnit,
                    ProfitMargin = p.ProfitMargin,
                    IsLowStock = p.IsLowStock
                });
            }

            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProductResponse>> GetProduct(string id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var product = await _unitOfWork.Products.GetByIdAsync(id);
            if (product == null || product.CreatedBy != userId)
                return NotFound();

            // Load related entities
            var category = product.Category ?? await _unitOfWork.Categories.GetByIdAsync(product.CategoryId);
            var supplier = product.Supplier;
            if (supplier == null && product.SupplierId != null)
            {
                supplier = await _unitOfWork.Suppliers.GetByIdAsync(product.SupplierId);
            }

            var response = new ProductResponse
            {
                Id = product.Id,
                Name = product.Name,
                Barcode = product.Barcode,
                CategoryId = product.CategoryId,
                CategoryName = category?.Name ?? "Unknown",
                BuyingPrice = product.BuyingPrice,
                SellingPrice = product.SellingPrice,
                CurrentStock = product.CurrentStock,
                MinStockLevel = product.MinStockLevel,
                SupplierId = product.SupplierId,
                SupplierName = supplier?.Name,
                CreatedAt = product.CreatedAt,
                IsActive = product.IsActive,
                ProfitPerUnit = product.ProfitPerUnit,
                ProfitMargin = product.ProfitMargin,
                IsLowStock = product.IsLowStock
            };

            return Ok(response);
        }

        [HttpPost]
        public async Task<ActionResult<ProductResponse>> CreateProduct(ProductCreateRequest request)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            // Verify category exists and belongs to user
            var category = await _unitOfWork.Categories.GetByIdAsync(request.CategoryId);
            if (category == null || category.CreatedBy != userId)
                return BadRequest("Invalid category.");

            var product = new Product
            {
                Name = request.Name,
                Barcode = request.Barcode,
                CategoryId = request.CategoryId,
                BuyingPrice = request.BuyingPrice,
                SellingPrice = request.SellingPrice,
                CurrentStock = request.CurrentStock,
                MinStockLevel = request.MinStockLevel,
                SupplierId = request.SupplierId,
                CreatedBy = userId,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Products.AddAsync(product);
            await _unitOfWork.CompleteAsync();

            var response = new ProductResponse
            {
                Id = product.Id,
                Name = product.Name,
                Barcode = product.Barcode,
                CategoryId = product.CategoryId,
                CategoryName = category.Name,
                BuyingPrice = product.BuyingPrice,
                SellingPrice = product.SellingPrice,
                CurrentStock = product.CurrentStock,
                MinStockLevel = product.MinStockLevel,
                SupplierId = product.SupplierId,
                CreatedAt = product.CreatedAt,
                IsActive = product.IsActive,
                ProfitPerUnit = product.ProfitPerUnit,
                ProfitMargin = product.ProfitMargin,
                IsLowStock = product.IsLowStock
            };

            return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, response);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ProductResponse>> UpdateProduct(string id, ProductCreateRequest request)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var product = await _unitOfWork.Products.GetByIdAsync(id);
            if (product == null || product.CreatedBy != userId)
                return NotFound();

            // Verify category exists and belongs to user
            var category = await _unitOfWork.Categories.GetByIdAsync(request.CategoryId);
            if (category == null || category.CreatedBy != userId)
                return BadRequest("Invalid category.");

            product.Name = request.Name;
            product.Barcode = request.Barcode;
            product.CategoryId = request.CategoryId;
            product.BuyingPrice = request.BuyingPrice;
            product.SellingPrice = request.SellingPrice;
            product.CurrentStock = request.CurrentStock;
            product.MinStockLevel = request.MinStockLevel;
            product.SupplierId = request.SupplierId;

            _unitOfWork.Products.Update(product);
            await _unitOfWork.CompleteAsync();

            var response = new ProductResponse
            {
                Id = product.Id,
                Name = product.Name,
                Barcode = product.Barcode,
                CategoryId = product.CategoryId,
                CategoryName = category.Name,
                BuyingPrice = product.BuyingPrice,
                SellingPrice = product.SellingPrice,
                CurrentStock = product.CurrentStock,
                MinStockLevel = product.MinStockLevel,
                SupplierId = product.SupplierId,
                CreatedAt = product.CreatedAt,
                IsActive = product.IsActive,
                ProfitPerUnit = product.ProfitPerUnit,
                ProfitMargin = product.ProfitMargin,
                IsLowStock = product.IsLowStock
            };

            return Ok(response);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteProduct(string id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var product = await _unitOfWork.Products.GetByIdAsync(id);
            if (product == null || product.CreatedBy != userId)
                return NotFound();

            // Soft delete
            product.IsActive = false;
            _unitOfWork.Products.Update(product);
            await _unitOfWork.CompleteAsync();

            return NoContent();
        }

        [HttpGet("low-stock")]
        public async Task<ActionResult<IEnumerable<ProductResponse>>> GetLowStockProducts()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var products = await _unitOfWork.Products.FindAsync(p =>
                p.CreatedBy == userId && p.IsActive && p.CurrentStock <= p.MinStockLevel);

            // Manually load Category and Supplier for each product
            var response = new List<ProductResponse>();
            foreach (var p in products)
            {
                var category = p.Category ?? await _unitOfWork.Categories.GetByIdAsync(p.CategoryId);
                var supplier = p.Supplier;
                if (supplier == null && p.SupplierId != null)
                {
                    supplier = await _unitOfWork.Suppliers.GetByIdAsync(p.SupplierId);
                }

                response.Add(new ProductResponse
                {
                    Id = p.Id,
                    Name = p.Name,
                    Barcode = p.Barcode,
                    CategoryId = p.CategoryId,
                    CategoryName = category?.Name ?? "Unknown",
                    BuyingPrice = p.BuyingPrice,
                    SellingPrice = p.SellingPrice,
                    CurrentStock = p.CurrentStock,
                    MinStockLevel = p.MinStockLevel,
                    SupplierId = p.SupplierId,
                    SupplierName = supplier?.Name,
                    CreatedAt = p.CreatedAt,
                    IsActive = p.IsActive,
                    ProfitPerUnit = p.ProfitPerUnit,
                    ProfitMargin = p.ProfitMargin,
                    IsLowStock = p.IsLowStock
                });
            }

            return Ok(response);
        }
    }
}