using Microsoft.EntityFrameworkCore;
using ShopManagement.Models.Entities;

namespace ShopManagement.API.Data.Context
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Sale> Sales { get; set; }
        public DbSet<SaleItem> SaleItems { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<ProductHistory> ProductHistories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {   
            base.OnModelCreating(modelBuilder);

            // REMOVE or COMMENT OUT the loop that sets singular table names
            // foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            // {
            //     modelBuilder.Entity(entityType.ClrType).ToTable(entityType.ClrType.Name);
            // }

            // User configuration
            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("Users"); // Explicitly set to "Users"
                entity.HasIndex(u => u.Email).IsUnique();
                entity.Property(u => u.CreatedAt).HasDefaultValueSql("UTC_TIMESTAMP()");
            });

            // Category configuration
            modelBuilder.Entity<Category>(entity =>
            {
                entity.ToTable("Categories");
                entity.HasOne(c => c.ParentCategory)
                    .WithMany(c => c.SubCategories)
                    .HasForeignKey(c => c.ParentCategoryId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(c => c.User)
                    .WithMany(u => u.Categories)
                    .HasForeignKey(c => c.CreatedBy)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.Property(c => c.CreatedAt).HasDefaultValueSql("UTC_TIMESTAMP()");
            });

            // Product configuration
            modelBuilder.Entity<Product>(entity =>
            {
                entity.ToTable("Products");
                entity.HasOne(p => p.Category)
                    .WithMany(c => c.Products)
                    .HasForeignKey(p => p.CategoryId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(p => p.Supplier)
                    .WithMany(s => s.Products)
                    .HasForeignKey(p => p.SupplierId)
                    .OnDelete(DeleteBehavior.SetNull);

                entity.HasOne(p => p.User)
                    .WithMany(u => u.Products)
                    .HasForeignKey(p => p.CreatedBy)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.Property(p => p.CreatedAt).HasDefaultValueSql("UTC_TIMESTAMP()");
                entity.Property(p => p.IsActive).HasDefaultValue(true);
                entity.Property(p => p.MinStockLevel).HasDefaultValue(10);
            });

            // Sale configuration
            modelBuilder.Entity<Sale>(entity =>
            {
                entity.ToTable("Sales");
                entity.HasOne(s => s.Customer)
                    .WithMany(c => c.Sales)
                    .HasForeignKey(s => s.CustomerId)
                    .OnDelete(DeleteBehavior.SetNull);

                entity.HasOne(s => s.User)
                    .WithMany(u => u.Sales)
                    .HasForeignKey(s => s.CreatedBy)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.Property(s => s.DateTime).HasDefaultValueSql("UTC_TIMESTAMP()");
                entity.Property(s => s.CreatedAt).HasDefaultValueSql("UTC_TIMESTAMP()");
                entity.Property(s => s.PaymentMethod).HasDefaultValue("cash");

                entity.HasIndex(s => s.DateTime);
                entity.HasIndex(s => s.CreatedBy);
            });

            // SaleItem configuration
            modelBuilder.Entity<SaleItem>(entity =>
            {
                entity.ToTable("SaleItems");
                entity.HasOne(si => si.Sale)
                    .WithMany(s => s.SaleItems)
                    .HasForeignKey(si => si.SaleId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(si => si.Product)
                    .WithMany(p => p.SaleItems)
                    .HasForeignKey(si => si.ProductId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Customer configuration
            modelBuilder.Entity<Customer>(entity =>
            {
                entity.ToTable("Customers");
                entity.HasOne(c => c.User)
                    .WithMany(u => u.Customers)
                    .HasForeignKey(c => c.CreatedBy)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.Property(c => c.CreatedAt).HasDefaultValueSql("UTC_TIMESTAMP()");
                entity.Property(c => c.TotalPurchases).HasDefaultValue(0);
                entity.Property(c => c.TotalTransactions).HasDefaultValue(0);
            });

            // Supplier configuration
            modelBuilder.Entity<Supplier>(entity =>
            {
                entity.ToTable("Suppliers");
                entity.HasOne(s => s.User)
                    .WithMany(u => u.Suppliers)
                    .HasForeignKey(s => s.CreatedBy)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.Property(s => s.CreatedAt).HasDefaultValueSql("UTC_TIMESTAMP()");
                entity.Property(s => s.TotalPurchases).HasDefaultValue(0);
                entity.Property(s => s.TotalProducts).HasDefaultValue(0);
            });

            // ProductHistory configuration
            modelBuilder.Entity<ProductHistory>(entity =>
            {
                entity.ToTable("ProductHistories");
            });
        }
    }
}