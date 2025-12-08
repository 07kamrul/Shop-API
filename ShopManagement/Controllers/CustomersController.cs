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
    public class CustomersController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public CustomersController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CustomerResponse>>> GetCustomers()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var customers = await _unitOfWork.Customers.FindAsync(c => c.CreatedBy == userId);

            var response = customers.Select(c => new CustomerResponse
            {
                Id = c.Id,
                Name = c.Name,
                Phone = c.Phone,
                Email = c.Email,
                Address = c.Address,
                TotalPurchases = c.TotalPurchases,
                TotalTransactions = c.TotalTransactions,
                LastPurchaseDate = c.LastPurchaseDate,
                CreatedAt = c.CreatedAt
            }).OrderByDescending(c => c.LastPurchaseDate);

            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CustomerResponse>> GetCustomer(string id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var customer = await _unitOfWork.Customers.GetByIdAsync(id);
            if (customer == null || customer.CreatedBy != userId)
                return NotFound();

            var response = new CustomerResponse
            {
                Id = customer.Id,
                Name = customer.Name,
                Phone = customer.Phone,
                Email = customer.Email,
                Address = customer.Address,
                TotalPurchases = customer.TotalPurchases,
                TotalTransactions = customer.TotalTransactions,
                LastPurchaseDate = customer.LastPurchaseDate,
                CreatedAt = customer.CreatedAt
            };

            return Ok(response);
        }

        [HttpPost]
        public async Task<ActionResult<CustomerResponse>> CreateCustomer(CustomerCreateRequest request)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var customer = new Customer
            {
                Name = request.Name,
                Phone = request.Phone,
                Email = request.Email,
                Address = request.Address,
                CreatedBy = userId,
                CreatedAt = DateTime.UtcNow,
                LastPurchaseDate = DateTime.UtcNow
            };

            await _unitOfWork.Customers.AddAsync(customer);
            await _unitOfWork.CompleteAsync();

            var response = new CustomerResponse
            {
                Id = customer.Id,
                Name = customer.Name,
                Phone = customer.Phone,
                Email = customer.Email,
                Address = customer.Address,
                TotalPurchases = customer.TotalPurchases,
                TotalTransactions = customer.TotalTransactions,
                LastPurchaseDate = customer.LastPurchaseDate,
                CreatedAt = customer.CreatedAt
            };

            return CreatedAtAction(nameof(GetCustomer), new { id = customer.Id }, response);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<CustomerResponse>> UpdateCustomer(string id, CustomerCreateRequest request)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var customer = await _unitOfWork.Customers.GetByIdAsync(id);
            if (customer == null || customer.CreatedBy != userId)
                return NotFound();

            customer.Name = request.Name;
            customer.Phone = request.Phone;
            customer.Email = request.Email;
            customer.Address = request.Address;

            _unitOfWork.Customers.Update(customer);
            await _unitOfWork.CompleteAsync();

            var response = new CustomerResponse
            {
                Id = customer.Id,
                Name = customer.Name,
                Phone = customer.Phone,
                Email = customer.Email,
                Address = customer.Address,
                TotalPurchases = customer.TotalPurchases,
                TotalTransactions = customer.TotalTransactions,
                LastPurchaseDate = customer.LastPurchaseDate,
                CreatedAt = customer.CreatedAt
            };

            return Ok(response);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteCustomer(string id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var customer = await _unitOfWork.Customers.GetByIdAsync(id);
            if (customer == null || customer.CreatedBy != userId)
                return NotFound();

            // Check if customer has sales
            var customerSales = await _unitOfWork.Sales.FindAsync(s => s.CustomerId == id);
            if (customerSales.Any())
                return BadRequest("Cannot delete customer with existing sales.");

            _unitOfWork.Customers.Remove(customer);
            await _unitOfWork.CompleteAsync();

            return NoContent();
        }

        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<CustomerResponse>>> SearchCustomers([FromQuery] string query)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var customers = await _unitOfWork.Customers.FindAsync(c =>
                c.CreatedBy == userId &&
                (c.Name.Contains(query) ||
                 (c.Phone != null && c.Phone.Contains(query)) ||
                 (c.Email != null && c.Email.Contains(query))));

            var response = customers.Select(c => new CustomerResponse
            {
                Id = c.Id,
                Name = c.Name,
                Phone = c.Phone,
                Email = c.Email,
                Address = c.Address,
                TotalPurchases = c.TotalPurchases,
                TotalTransactions = c.TotalTransactions,
                LastPurchaseDate = c.LastPurchaseDate,
                CreatedAt = c.CreatedAt
            });

            return Ok(response);
        }

        [HttpGet("top")]
        public async Task<ActionResult<IEnumerable<CustomerResponse>>> GetTopCustomers([FromQuery] int limit = 10)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var customers = await _unitOfWork.Customers.FindAsync(c => c.CreatedBy == userId);

            var response = customers
                .OrderByDescending(c => c.TotalPurchases)
                .Take(limit)
                .Select(c => new CustomerResponse
                {
                    Id = c.Id,
                    Name = c.Name,
                    Phone = c.Phone,
                    Email = c.Email,
                    Address = c.Address,
                    TotalPurchases = c.TotalPurchases,
                    TotalTransactions = c.TotalTransactions,
                    LastPurchaseDate = c.LastPurchaseDate,
                    CreatedAt = c.CreatedAt
                });

            return Ok(response);
        }
    }
}
