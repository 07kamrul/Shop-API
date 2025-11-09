using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using ShopManagement.Models.Entities;

namespace ShopManagement.Data.Context
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

            // User configuration
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasIndex(u => u.Email).IsUnique();
                entity.Property(u => u.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(u => u.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            });

            // Category configuration
            modelBuilder.Entity<Category>(entity =>
            {
                entity.HasOne(c => c.ParentCategory)
                      .WithMany(c => c.SubCategories)
                      .HasForeignKey(c => c.ParentCategoryId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(c => c.User)
                      .WithMany(u => u.Categories)
                      .HasForeignKey(c => c.CreatedBy)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // Product configuration
            modelBuilder.Entity<Product>(entity =>
            {
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

                entity.HasIndex(p => new { p.CreatedBy, p.IsActive });
            });

            // Sale configuration
            modelBuilder.Entity<Sale>(entity =>
            {
                entity.HasOne(s => s.Customer)
                      .WithMany(c => c.Sales)
                      .HasForeignKey(s => s.CustomerId)
                      .OnDelete(DeleteBehavior.SetNull);

                entity.HasOne(s => s.User)
                      .WithMany(u => u.Sales)
                      .HasForeignKey(s => s.CreatedBy)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(s => new { s.CreatedBy, s.DateTime });
            });

            // SaleItem configuration
            modelBuilder.Entity<SaleItem>(entity =>
            {
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
                entity.HasOne(c => c.User)
                      .WithMany(u => u.Customers)
                      .HasForeignKey(c => c.CreatedBy)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // Supplier configuration
            modelBuilder.Entity<Supplier>(entity =>
            {
                entity.HasOne(s => s.User)
                      .WithMany(u => u.Suppliers)
                      .HasForeignKey(s => s.CreatedBy)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // ProductHistory configuration
            modelBuilder.Entity<ProductHistory>(entity =>
            {
                entity.HasOne(ph => ph.Product)
                      .WithMany(p => p.ProductHistories)
                      .HasForeignKey(ph => ph.ProductId)
                      .OnDelete(DeleteBehavior.Cascade);
            });
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var entries = ChangeTracker.Entries()
                .Where(e => e.Entity is BaseEntity && (
                        e.State == EntityState.Added
                        || e.State == EntityState.Modified));

            foreach (var entityEntry in entries)
            {
                if (entityEntry.State == EntityState.Added)
                {
                    ((BaseEntity)entityEntry.Entity).CreatedAt = DateTime.UtcNow;
                }

                ((BaseEntity)entityEntry.Entity).UpdatedAt = DateTime.UtcNow;
            }

            return await base.SaveChangesAsync(cancellationToken);
        }
    }
}
