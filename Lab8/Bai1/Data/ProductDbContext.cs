using Microsoft.EntityFrameworkCore;
using Bai1.Models;

namespace Bai1.Data
{
  public class ProductDbContext : DbContext
    {
        public ProductDbContext(DbContextOptions<ProductDbContext> options) : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>().HasData(
                new Product { Id = 1, Name = "Trà sữa truyền thống", Price = 25000, Description = "Vị ngon đậm đà" },
                new Product { Id = 2, Name = "Trà đào cam sả", Price = 30000, Description = "Thanh mát" }
            );
        }
    }
}