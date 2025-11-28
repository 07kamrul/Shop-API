// Controllers/InventoryController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShopManagement.Models.DTOs.Inventory;
using ShopManagement.Services;
using System.Security.Claims;

namespace ShopManagement.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class InventoryController : ControllerBase
    {
        private readonly IInventoryService _inventoryService;

        public InventoryController(IInventoryService inventoryService)
        {
            _inventoryService = inventoryService;
        }

        private string GetUserId() =>
            User.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? throw new UnauthorizedAccessException();

        [HttpGet("summary")]
        public async Task<ActionResult<InventorySummaryDto>> GetSummary()
        {
            var userId = GetUserId();
            var summary = await _inventoryService.GetInventorySummaryAsync(userId);
            return Ok(summary);
        }

        [HttpGet("alerts")]
        public async Task<ActionResult<List<StockAlertDto>>> GetStockAlerts()
        {
            var userId = GetUserId();
            var alerts = await _inventoryService.GetStockAlertsAsync(userId);
            return Ok(alerts);
        }

        [HttpGet("by-category")]
        public async Task<ActionResult<List<CategoryInventoryDto>>> GetCategoryInventory()
        {
            var userId = GetUserId();
            var data = await _inventoryService.GetCategoryInventoryAsync(userId);
            return Ok(data);
        }

        [HttpGet("restock-needed")]
        public async Task<ActionResult<List<Product>>> GetProductsNeedingRestock()
        {
            var userId = GetUserId();
            var products = await _inventoryService.GetProductsNeedingRestockAsync(userId);
            return Ok(products);
        }

        [HttpGet("turnover")]
        public async Task<ActionResult<decimal>> GetInventoryTurnover(
            [FromQuery] DateTime startDate,
            [FromQuery] DateTime endDate)
        {
            var userId = GetUserId();
            var turnover = await _inventoryService.CalculateInventoryTurnoverAsync(userId, startDate, endDate);
            return Ok(turnover);
        }
    }
}