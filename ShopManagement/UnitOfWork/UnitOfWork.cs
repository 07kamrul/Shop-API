using Microsoft.EntityFrameworkCore.Storage;
using ShopManagement.API.Data.Context;
using ShopManagement.Data.Repositories;
using ShopManagement.Interfaces;
using ShopManagement.Models.Entities;

namespace ShopManagement.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
            Users = new GenericRepository<User>(_context);
            Categories = new GenericRepository<Category>(_context);
            Products = new GenericRepository<Product>(_context);
            Sales = new GenericRepository<Sale>(_context);
            SaleItems = new GenericRepository<SaleItem>(_context);
            Customers = new GenericRepository<Customer>(_context);
            Suppliers = new GenericRepository<Supplier>(_context);
            ProductHistories = new GenericRepository<ProductHistory>(_context);
        }

        public IGenericRepository<User> Users { get; private set; }
        public IGenericRepository<Category> Categories { get; private set; }
        public IGenericRepository<Product> Products { get; private set; }
        public IGenericRepository<Sale> Sales { get; private set; }
        public IGenericRepository<SaleItem> SaleItems { get; private set; }
        public IGenericRepository<Customer> Customers { get; private set; }
        public IGenericRepository<Supplier> Suppliers { get; private set; }
        public IGenericRepository<ProductHistory> ProductHistories { get; private set; }

        public async Task<int> CompleteAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public int Complete()
        {
            return _context.SaveChanges();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
