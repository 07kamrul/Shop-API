using ShopManagement.Models.Entities;
using System;
using System.Threading.Tasks;
namespace ShopManagement.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IGenericRepository<User> Users { get; }
        IGenericRepository<Category> Categories { get; }
        IGenericRepository<Product> Products { get; }
        IGenericRepository<Sale> Sales { get; }
        IGenericRepository<SaleItem> SaleItems { get; }
        IGenericRepository<Customer> Customers { get; }
        IGenericRepository<Supplier> Suppliers { get; }
        IGenericRepository<ProductHistory> ProductHistories { get; }

        Task<int> CompleteAsync();
        int Complete();
    }
}
