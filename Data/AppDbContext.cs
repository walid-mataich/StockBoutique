using Microsoft.EntityFrameworkCore;
using StockBoutique.Models;

namespace StockBoutique.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Product> Products { get; set; } = null!;
        public DbSet<Category> Categories { get; set; } = null!;
        public DbSet<StockTransaction> StockTransactions { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Default categories
            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Électronique" },
                new Category { Id = 2, Name = "Vêtements" },
                new Category { Id = 3, Name = "Alimentation" },
                new Category { Id = 4, Name = "Maison" }
            );
        }
    }
}
