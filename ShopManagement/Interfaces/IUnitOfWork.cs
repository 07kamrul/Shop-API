namespace ShopManagement.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IUserRepository Users { get; }
        ICategoryRepository Categories { get; }
        IProductRepository Products { get; }
        ISaleRepository Sales { get; }
        ICustomerRepository Customers { get; }
        ISupplierRepository Suppliers { get; }
        IProductHistoryRepository ProductHistories { get; }

        Task<int> SaveChangesAsync();
        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
    }
}
