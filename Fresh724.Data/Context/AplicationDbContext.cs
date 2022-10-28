using System.Reflection;
using Fresh724.Entity.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;


namespace Fresh724.Data.Context;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }
    
    //public DbSet<Product> Products { get; set; }
    
    public DbSet<Category> Categories => Set<Category>();
    
    //public DbSet<Image> Images { get; set; }
    
    public DbSet<Posts> Posts => Set<Posts>();
    public DbSet<AddressUser> Addresses => Set<AddressUser>();
    public DbSet<Employee> Employees => Set<Employee>();
    public DbSet<Product> Products => Set<Product>();
    
    public DbSet<Company> Companies => Set<Company>();
    public DbSet<ShoppingCart> ShoppingCarts => Set<ShoppingCart>();
   
 
    
}