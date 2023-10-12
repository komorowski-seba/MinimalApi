using Infrastructure.Persistence;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;

namespace Infrastructure;

public static class InfrastructureExtension
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddPersistenceServices(configuration);
        return services;
    }

    public static IApplicationBuilder UseInfrastructureConfiguration(this IApplicationBuilder app)
    {
        app.UsePersistenceConfiguration();
        return app;
    }
}