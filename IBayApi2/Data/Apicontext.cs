using IBayApi2.Models;
using Microsoft.EntityFrameworkCore;

namespace IBayApi2.Data;

public class ApiContext : DbContext
{
    public ApiContext(DbContextOptions<ApiContext> options) : base(options)
    {
        
    }

    public DbSet<Product> Product { get; set; }
    public DbSet<User> Users { get; set; }
}