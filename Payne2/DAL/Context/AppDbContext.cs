using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Payne.Models;

namespace Payne.DAL.Context;

public class AppDbContext : IdentityDbContext<AppUser>
{
    public AppDbContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<Tags> Tags { get; set; }
    public DbSet<TagProduct> TagProducts { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<Review> Reviews { get; set; }
    public DbSet<Slider> Sliders { get; set; }
    public DbSet<ProductImages> ProductImages { get; set; }
}