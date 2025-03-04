using Annotations.Core.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace Annotations.API;

public class Startup(IConfiguration configuration)
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

    public Task Configure(WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        using (var scope = app.Services.CreateScope())
        {
            using var context = scope.ServiceProvider.GetRequiredService<AnnotationsDbContext>();

            context.Database.Migrate();
            
            if (!context.Users.Any())
            {
                context.Add(new Admin
                {
                    UserId = 0,
                    FirstName = "Admin",
                    LastName = "Adminsen"
                });
                context.SaveChanges();
            }
        }

        app.UseStaticFiles();
        app.UseRouting();
        app.UseAuthorization();
        app.UseHttpsRedirection();

        app.MapControllers();
        return Task.FromResult(Task.CompletedTask);
    }

    
}