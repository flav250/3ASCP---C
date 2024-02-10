using IBayApi2.Models;
using Microsoft.EntityFrameworkCore;

namespace IBayApi2.Data;

public class ApiContext : DbContext
{
    public ApiContext(DbContextOptions<ApiContext> options) : base(options)
    {
        
    }

    public DbSet<product> ProductItems { get; set; } = null!;
    public DbSet<user> Users { get; set; } = null!;
}