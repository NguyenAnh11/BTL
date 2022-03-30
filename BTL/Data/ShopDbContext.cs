using BTL.Models.Interfaces;
using System;
using System.Data.Entity;
using System.Threading;
using System.Threading.Tasks;
using BTL.Models;

namespace BTL.Data
{
    public class ShopDbContext : DbContext
    {
        public DbSet<Category> Categories { get; set; }
        public DbSet<CategoryProduct> CategoryProducts { get; set; }
        public DbSet<CategoryManufacturer> CategoryManufacturers { get; set; }
        public DbSet<Manufacturer> Manufacturers { get; set; }   
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<UserPassword> UserPasswords { get; set; }
        public ShopDbContext() : base("Shop") 
        {
            
        }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<CategoryProduct>().HasKey(p => new { p.CategoryId, p.ProductId });
            modelBuilder.Entity<CategoryManufacturer>().HasKey(p => new { p.CategoryId, p.ManufacturerId });
            modelBuilder.Entity<OrderItem>().HasKey(p => new { p.OrderId, p.ProductId });
        }
        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken)
        {
            foreach (var entry in ChangeTracker.Entries())
            {
                if (typeof(ISoftDeletedEntity).IsAssignableFrom(entry.GetType()))
                {
                    switch (entry.State)
                    {
                        case EntityState.Added:
                            entry.Property("IsDeleted").CurrentValue = false;
                            break;
                        case EntityState.Deleted:
                            entry.Property("IsDeleted").CurrentValue = true;
                            break;
                        default:
                            entry.Property("IsDeleted").CurrentValue = entry.Property("IsDeleted").OriginalValue;
                            break;
                    }
                }

                if (typeof(IDateCreatedEntity).IsAssignableFrom(entry.GetType()))
                {
                    switch (entry.State)
                    {
                        case EntityState.Added:
                            entry.Property("CreateAt").CurrentValue = DateTime.Now;
                            break;
                    }
                }

                if (typeof(IDateUpdatedEntity).IsAssignableFrom(entry.GetType()))
                {
                    switch (entry.State)
                    {
                        case EntityState.Modified:
                            entry.Property("UpdateAt").CurrentValue = DateTime.Now;
                            break;
                    }
                }
            }

            return base.SaveChangesAsync(cancellationToken);
        }
    }
}