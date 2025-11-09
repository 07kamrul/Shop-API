using Microsoft.EntityFrameworkCore.Storage;
using ShopManagement.Interfaces;

namespace ShopManagement.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        private IDbContextTransaction? _transaction;

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
            Users = new UserRepository(_context);
            Categories = new CategoryRepository(_context);
            Products = new ProductRepository(_context);
            Sales = new SaleRepository(_context);
            Customers = new CustomerRepository(_context);
            Suppliers = new SupplierRepository(_context);
            ProductHistories = new ProductHistoryRepository(_context);
        }

        public IUserRepository Users { get; private set; }
        public ICategoryRepository Categories { get; private set; }
        public IProductRepository Products { get; private set; }
        public ISaleRepository Sales { get; private set; }
        public ICustomerRepository Customers { get; private set; }
        public ISupplierRepository Suppliers { get; private set; }
        public IProductHistoryRepository ProductHistories { get; private set; }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public async Task BeginTransactionAsync()
        {
            _transaction = await _context.Database.BeginTransactionAsync();
        }

        public async Task CommitTransactionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.CommitAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public async Task RollbackTransactionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.RollbackAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public void Dispose()
        {
            _transaction?.Dispose();
            _context.Dispose();
        }
    }
}
