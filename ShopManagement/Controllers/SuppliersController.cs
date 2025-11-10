using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShopManagement.Interfaces;
using ShopManagement.Models.DTOs;
using ShopManagement.Models.Entities;
using System.Security.Claims;

namespace ShopManagement.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class SuppliersController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public SuppliersController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<SupplierResponse>>> GetSuppliers()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var suppliers = await _unitOfWork.Suppliers.FindAsync(s => s.CreatedBy == userId);

            var response = suppliers.Select(s => new SupplierResponse
            {
                Id = s.Id,
                Name = s.Name,
                ContactPerson = s.ContactPerson,
                Phone = s.Phone,
                Email = s.Email,
                Address = s.Address,
                TotalPurchases = s.TotalPurchases,
                TotalProducts = s.TotalProducts,
                LastPurchaseDate = s.LastPurchaseDate,
                CreatedAt = s.CreatedAt
            }).OrderByDescending(s => s.LastPurchaseDate);

            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<SupplierResponse>> GetSupplier(string id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var supplier = await _unitOfWork.Suppliers.GetByIdAsync(id);
            if (supplier == null || supplier.CreatedBy != userId)
                return NotFound();

            var response = new SupplierResponse
            {
                Id = supplier.Id,
                Name = supplier.Name,
                ContactPerson = supplier.ContactPerson,
                Phone = supplier.Phone,
                Email = supplier.Email,
                Address = supplier.Address,
                TotalPurchases = supplier.TotalPurchases,
                TotalProducts = supplier.TotalProducts,
                LastPurchaseDate = supplier.LastPurchaseDate,
                CreatedAt = supplier.CreatedAt
            };

            return Ok(response);
        }

        [HttpPost]
        public async Task<ActionResult<SupplierResponse>> CreateSupplier(SupplierCreateRequest request)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var supplier = new Supplier
            {
                Name = request.Name,
                ContactPerson = request.ContactPerson,
                Phone = request.Phone,
                Email = request.Email,
                Address = request.Address,
                CreatedBy = userId,
                CreatedAt = DateTime.UtcNow,
                LastPurchaseDate = DateTime.UtcNow
            };

            await _unitOfWork.Suppliers.AddAsync(supplier);
            await _unitOfWork.CompleteAsync();

            var response = new SupplierResponse
            {
                Id = supplier.Id,
                Name = supplier.Name,
                ContactPerson = supplier.ContactPerson,
                Phone = supplier.Phone,
                Email = supplier.Email,
                Address = supplier.Address,
                TotalPurchases = supplier.TotalPurchases,
                TotalProducts = supplier.TotalProducts,
                LastPurchaseDate = supplier.LastPurchaseDate,
                CreatedAt = supplier.CreatedAt
            };

            return CreatedAtAction(nameof(GetSupplier), new { id = supplier.Id }, response);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<SupplierResponse>> UpdateSupplier(string id, SupplierCreateRequest request)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var supplier = await _unitOfWork.Suppliers.GetByIdAsync(id);
            if (supplier == null || supplier.CreatedBy != userId)
                return NotFound();

            supplier.Name = request.Name;
            supplier.ContactPerson = request.ContactPerson;
            supplier.Phone = request.Phone;
            supplier.Email = request.Email;
            supplier.Address = request.Address;

            _unitOfWork.Suppliers.Update(supplier);
            await _unitOfWork.CompleteAsync();

            var response = new SupplierResponse
            {
                Id = supplier.Id,
                Name = supplier.Name,
                ContactPerson = supplier.ContactPerson,
                Phone = supplier.Phone,
                Email = supplier.Email,
                Address = supplier.Address,
                TotalPurchases = supplier.TotalPurchases,
                TotalProducts = supplier.TotalProducts,
                LastPurchaseDate = supplier.LastPurchaseDate,
                CreatedAt = supplier.CreatedAt
            };

            return Ok(response);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteSupplier(string id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var supplier = await _unitOfWork.Suppliers.GetByIdAsync(id);
            if (supplier == null || supplier.CreatedBy != userId)
                return NotFound();

            // Check if supplier has products
            var supplierProducts = await _unitOfWork.Products.FindAsync(p => p.SupplierId == id && p.IsActive);
            if (supplierProducts.Any())
                return BadRequest("Cannot delete supplier with existing products.");

            _unitOfWork.Suppliers.Remove(supplier);
            await _unitOfWork.CompleteAsync();

            return NoContent();
        }

        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<SupplierResponse>>> SearchSuppliers([FromQuery] string query)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var suppliers = await _unitOfWork.Suppliers.FindAsync(s =>
                s.CreatedBy == userId &&
                (s.Name.Contains(query) ||
                 (s.ContactPerson != null && s.ContactPerson.Contains(query)) ||
                 (s.Phone != null && s.Phone.Contains(query)) ||
                 (s.Email != null && s.Email.Contains(query))));

            var response = suppliers.Select(s => new SupplierResponse
            {
                Id = s.Id,
                Name = s.Name,
                ContactPerson = s.ContactPerson,
                Phone = s.Phone,
                Email = s.Email,
                Address = s.Address,
                TotalPurchases = s.TotalPurchases,
                TotalProducts = s.TotalProducts,
                LastPurchaseDate = s.LastPurchaseDate,
                CreatedAt = s.CreatedAt
            });

            return Ok(response);
        }

        [HttpGet("top")]
        public async Task<ActionResult<IEnumerable<SupplierResponse>>> GetTopSuppliers([FromQuery] int limit = 10)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var suppliers = await _unitOfWork.Suppliers.FindAsync(s => s.CreatedBy == userId);

            var response = suppliers
                .OrderByDescending(s => s.TotalPurchases)
                .Take(limit)
                .Select(s => new SupplierResponse
                {
                    Id = s.Id,
                    Name = s.Name,
                    ContactPerson = s.ContactPerson,
                    Phone = s.Phone,
                    Email = s.Email,
                    Address = s.Address,
                    TotalPurchases = s.TotalPurchases,
                    TotalProducts = s.TotalProducts,
                    LastPurchaseDate = s.LastPurchaseDate,
                    CreatedAt = s.CreatedAt
                });

            return Ok(response);
        }
    }
}
