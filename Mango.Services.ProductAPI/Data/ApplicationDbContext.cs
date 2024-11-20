using Mango.Services.ProductAPI.Modals;
using Microsoft.EntityFrameworkCore;

namespace Mango.Services.ProductAPI.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Product>().HasData(new Product 
            { ProductId = 1, Name = "Samosa", Price = 40, Description = "Very delicious but not good for health", 
              ImageUrl = "https://placehold.co/603x403", CategoryName = "Appetizer" 
            });
            
            modelBuilder.Entity<Product>().HasData(new Product 
            { ProductId = 2, Name = "Paneer Tikka", Price = 70, Description = "Very delicious very juicy, good for healt if not very spicy", 
              ImageUrl = "https://placehold.co/602x402", CategoryName = "Appetizer" 
            });
            
            modelBuilder.Entity<Product>().HasData(new Product 
            { ProductId = 3, Name = "Sweet Pie", Price = 70, Description = "Very delicious and sweet", 
              ImageUrl = "https://placehold.co/601x401", CategoryName = "Dessert" 
            });

            modelBuilder.Entity<Product>().HasData(new Product 
            { ProductId = 4, Name = "Alo Wala Naan", Price = 60, Description = "Very delicious and little bit of spicy, good for sunday breakfast", 
              ImageUrl = "https://placehold.co/600x400", CategoryName = "Entree" 
            });
        }
    }
}
