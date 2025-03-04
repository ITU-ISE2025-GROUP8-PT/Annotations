using Annotations.Core;

using Microsoft.AspNetCore.Identity;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace Annotations.API;

public class Startup(IConfiguration configuration, IHostEnvironment environment)
{
    public void ConfigureServices(IServiceCollection services)
    {
        // Add services to container
        services.AddSwaggerGen();
        services.AddControllers();

        var connectionString =
            configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection string"
                + "'DefaultConnection' not found.");
                
        services.AddDbContext<AnnotationsDbContext>(options =>
        {
            options.UseSqlite(connectionString);
        });
    }

    public async Task Configure(WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        using (var scope = app.Services.CreateScope())
        {
            using var context = scope.ServiceProvider.GetRequiredService<AnnotationsDbContext>();

            /*context.Database.Migrate();

            // Seeds with default dataset if starting with an empty DB.
            if (!context.Authors.Any() && !context.Cheeps.Any())
            {
                var initializer = new DbInitializer(context, userManager);
                await initializer.SeedDatabase();
            }
            */
        }

        app.UseStaticFiles();
        app.UseRouting();
        app.UseAuthorization();
        app.UseHttpsRedirection();

        app.MapControllers();
    }

    
}