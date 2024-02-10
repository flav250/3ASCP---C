using IBayApi2.Data;
using Microsoft.EntityFrameworkCore;

namespace IBayApi2;

public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddControllers();
        services.AddEndpointsApiExplorer();
        services.AddDbContext<ApiContext>(opt => opt.UseSqlServer("Server=http://localhost:1433;Database=IBayApi_db;Trusted_Connection=True;"));
        services.AddSwaggerGen();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseRouting();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });

        if (env.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();
    }
}