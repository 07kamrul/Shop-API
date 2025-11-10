using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShopManagement.Models.DTOs;
using ShopManagement.Models.Entities;
using ShopManagement.Services;
using System.Security.Claims;

namespace ShopManagement.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class SalesController : ControllerBase
    {
        private readonly ISaleService _saleService;

        public SalesController(ISaleService saleService)
        {
            _saleService = saleService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<SaleResponse>>> GetSales(
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var sales = await _saleService.GetSalesAsync(userId, startDate, endDate);
            return Ok(sales);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<SaleResponse>> GetSale(string id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var sale = await _saleService.GetSaleByIdAsync(id, userId);
            if (sale == null)
                return NotFound();

            return Ok(sale);
        }

        [HttpPost]
        public async Task<ActionResult<SaleResponse>> CreateSale(SaleCreateRequest request)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            try
            {
                var sale = await _saleService.CreateSaleAsync(request, userId);
                return CreatedAtAction(nameof(GetSale), new { id = sale.Id }, sale);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while creating the sale.");
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteSale(string id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var result = await _saleService.DeleteSaleAsync(id, userId);
            if (!result)
                return NotFound();

            return NoContent();
        }

        [HttpGet("today")]
        public async Task<ActionResult<IEnumerable<SaleResponse>>> GetTodaySales()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var sales = await _saleService.GetTodaySalesAsync(userId);
            return Ok(sales);
        }
    }
}
