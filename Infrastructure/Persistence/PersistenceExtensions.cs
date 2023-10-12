using Application.Interfaces;
using Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;

namespace Infrastructure.Persistence;

public static class PersistenceExtensions
{
    public static IServiceCollection AddPersistenceServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(opt => opt.UseSqlServer(configuration.GetConnectionString("DBConnection")));
        services.AddScoped<IApplicationDbContext>(provider => provider.GetService<ApplicationDbContext>()!);
        services.AddScoped<IAuthorRepository, AuthorRepository>();
        return services;
    }
    
    public static IApplicationBuilder UsePersistenceConfiguration(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices
            .GetRequiredService<IServiceScopeFactory>()
            .CreateScope();
        var dbContext = scope.ServiceProvider.GetService<ApplicationDbContext>();
        dbContext?.Database.Migrate();
        
        return app;
    }
}
