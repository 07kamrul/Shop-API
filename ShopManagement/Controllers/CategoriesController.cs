using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShopManagement.Interfaces;
using ShopManagement.Models.DTOs;
using ShopManagement.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ShopManagement.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriesController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public CategoriesController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CategoryResponse>>> GetCategories()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var categories = await _unitOfWork.Categories.FindAsync(c => c.CreatedBy == userId);
            var products = await _unitOfWork.Products.FindAsync(p => p.CreatedBy == userId && p.IsActive);

            var response = categories.Select(c => new CategoryResponse
            {
                Id = c.Id,
                Name = c.Name,
                ParentCategoryId = c.ParentCategoryId,
                ParentCategoryName = c.ParentCategory?.Name,
                Description = c.Description,
                ProfitMarginTarget = c.ProfitMarginTarget,
                CreatedAt = c.CreatedAt,
                ProductCount = products.Count(p => p.CategoryId == c.Id),
                SubCategories = categories.Where(sc => sc.ParentCategoryId == c.Id)
                    .Select(sc => new CategoryResponse
                    {
                        Id = sc.Id,
                        Name = sc.Name,
                        ParentCategoryId = sc.ParentCategoryId,
                        Description = sc.Description,
                        ProfitMarginTarget = sc.ProfitMarginTarget,
                        CreatedAt = sc.CreatedAt,
                        ProductCount = products.Count(p => p.CategoryId == sc.Id)
                    })
            });

            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CategoryResponse>> GetCategory(string id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var category = await _unitOfWork.Categories.GetByIdAsync(id);
            if (category == null || category.CreatedBy != userId)
                return NotFound();

            var products = await _unitOfWork.Products.FindAsync(p => p.CategoryId == id && p.IsActive);
            var subCategories = await _unitOfWork.Categories.FindAsync(c => c.ParentCategoryId == id);

            var response = new CategoryResponse
            {
                Id = category.Id,
                Name = category.Name,
                ParentCategoryId = category.ParentCategoryId,
                ParentCategoryName = category.ParentCategory?.Name,
                Description = category.Description,
                ProfitMarginTarget = category.ProfitMarginTarget,
                CreatedAt = category.CreatedAt,
                ProductCount = products.Count(),
                SubCategories = subCategories.Select(sc => new CategoryResponse
                {
                    Id = sc.Id,
                    Name = sc.Name,
                    ParentCategoryId = sc.ParentCategoryId,
                    Description = sc.Description,
                    ProfitMarginTarget = sc.ProfitMarginTarget,
                    CreatedAt = sc.CreatedAt,
                    ProductCount = products.Count(p => p.CategoryId == sc.Id)
                })
            };

            return Ok(response);
        }

        [HttpPost]
        public async Task<ActionResult<CategoryResponse>> CreateCategory(CategoryCreateRequest request)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            // Validate parent category
            if (!string.IsNullOrEmpty(request.ParentCategoryId))
            {
                var parentCategory = await _unitOfWork.Categories.GetByIdAsync(request.ParentCategoryId);
                if (parentCategory == null || parentCategory.CreatedBy != userId)
                    return BadRequest("Invalid parent category.");
            }

            var category = new Category
            {
                Name = request.Name,
                ParentCategoryId = request.ParentCategoryId,
                Description = request.Description,
                ProfitMarginTarget = request.ProfitMarginTarget,
                CreatedBy = userId,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Categories.AddAsync(category);
            await _unitOfWork.CompleteAsync();

            var response = new CategoryResponse
            {
                Id = category.Id,
                Name = category.Name,
                ParentCategoryId = category.ParentCategoryId,
                Description = category.Description,
                ProfitMarginTarget = category.ProfitMarginTarget,
                CreatedAt = category.CreatedAt,
                ProductCount = 0
            };

            return CreatedAtAction(nameof(GetCategory), new { id = category.Id }, response);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<CategoryResponse>> UpdateCategory(string id, CategoryCreateRequest request)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var category = await _unitOfWork.Categories.GetByIdAsync(id);
            if (category == null || category.CreatedBy != userId)
                return NotFound();

            // Validate parent category (cannot be self or create circular reference)
            if (!string.IsNullOrEmpty(request.ParentCategoryId))
            {
                if (request.ParentCategoryId == id)
                    return BadRequest("Category cannot be its own parent.");

                var parentCategory = await _unitOfWork.Categories.GetByIdAsync(request.ParentCategoryId);
                if (parentCategory == null || parentCategory.CreatedBy != userId)
                    return BadRequest("Invalid parent category.");

                // Check for circular reference
                var currentParent = parentCategory;
                while (currentParent != null)
                {
                    if (currentParent.ParentCategoryId == id)
                        return BadRequest("Circular reference detected.");
                    currentParent = await _unitOfWork.Categories.GetByIdAsync(currentParent.ParentCategoryId);
                }
            }

            category.Name = request.Name;
            category.ParentCategoryId = request.ParentCategoryId;
            category.Description = request.Description;
            category.ProfitMarginTarget = request.ProfitMarginTarget;

            _unitOfWork.Categories.Update(category);
            await _unitOfWork.CompleteAsync();

            var products = await _unitOfWork.Products.FindAsync(p => p.CategoryId == id && p.IsActive);
            var response = new CategoryResponse
            {
                Id = category.Id,
                Name = category.Name,
                ParentCategoryId = category.ParentCategoryId,
                ParentCategoryName = category.ParentCategory?.Name,
                Description = category.Description,
                ProfitMarginTarget = category.ProfitMarginTarget,
                CreatedAt = category.CreatedAt,
                ProductCount = products.Count()
            };

            return Ok(response);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteCategory(string id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var category = await _unitOfWork.Categories.GetByIdAsync(id);
            if (category == null || category.CreatedBy != userId)
                return NotFound();

            // Check if category has products
            var products = await _unitOfWork.Products.FindAsync(p => p.CategoryId == id && p.IsActive);
            if (products.Any())
                return BadRequest("Cannot delete category with existing products.");

            // Check if category has subcategories
            var subCategories = await _unitOfWork.Categories.FindAsync(c => c.ParentCategoryId == id);
            if (subCategories.Any())
                return BadRequest("Cannot delete category with subcategories.");

            _unitOfWork.Categories.Remove(category);
            await _unitOfWork.CompleteAsync();

            return NoContent();
        }
    }
}
