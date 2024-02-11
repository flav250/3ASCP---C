using IBayApi2.Models;
using Microsoft.EntityFrameworkCore;

namespace IBayApi2.Data;

public class ApiContext : DbContext
{
    public ApiContext(DbContextOptions<ApiContext> options) : base(options)
    {
    }

    public DbSet<Product> Product { get; set; }
    public DbSet<Member> Member { get; set; }
    public DbSet<Cart> Cart { get; set; }
    public DbSet<CartItem> CartItem { get; set; }
}