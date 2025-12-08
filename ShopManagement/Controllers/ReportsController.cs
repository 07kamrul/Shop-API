using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShopManagement.Models.DTOs;
using ShopManagement.Models.Entities;
using ShopManagement.Services;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ShopManagement.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ReportsController : ControllerBase
    {
        private readonly IReportService _reportService;

        public ReportsController(IReportService reportService)
        {
            _reportService = reportService;
        }

        [HttpGet("profit-loss")]
        public async Task<ActionResult<ProfitLossReport>> GetProfitLossReport(
            [FromQuery] DateTime startDate,
            [FromQuery] DateTime endDate)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            try
            {
                var report = await _reportService.GetProfitLossReportAsync(userId, startDate, endDate);
                return Ok(report);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while generating the report.");
            }
        }

        [HttpGet("daily-sales")]
        public async Task<ActionResult<IEnumerable<DailySalesReport>>> GetDailySalesReport(
            [FromQuery] DateTime startDate,
            [FromQuery] DateTime endDate)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            try
            {
                var reports = await _reportService.GetDailySalesReportAsync(userId, startDate, endDate);
                return Ok(reports);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while generating the report.");
            }
        }

        [HttpGet("top-products")]
        public async Task<ActionResult<IEnumerable<ProductSales>>> GetTopSellingProducts(
            [FromQuery] DateTime startDate,
            [FromQuery] DateTime endDate,
            [FromQuery] int limit = 10)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            try
            {
                var products = await _reportService.GetTopSellingProductsAsync(userId, startDate, endDate, limit);
                return Ok(products);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while generating the report.");
            }
        }
    }
}
