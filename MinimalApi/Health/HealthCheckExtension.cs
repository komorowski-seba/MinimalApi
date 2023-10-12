using HealthChecks.UI.Client;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace MinimalApi.Health;

public static class HealthCheckExtension
{
    private const string HealthCheckEndpointName = "/_health";
    private const string HealthCheckEndpointDashboardName = "/_healthDashboard";
    
    public static IServiceCollection AddHealthCheckServices(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddHealthChecks()
            .AddCheck<ApiHealthCheck>("Api", tags: new [] {"Api"})
            .AddSqlServer(
                configuration.GetConnectionString("DBConnection") ?? throw new NullReferenceException("Db connection string is null"),
                healthQuery: "SELECT 1;",
                name: "Db Server",
                failureStatus: HealthStatus.Degraded,
                tags: new[] { "Db", "DbServer" })
            .AddRedis(
                configuration.GetConnectionString("Redis") ?? throw new NullReferenceException("Redis connection string is null"),
                name: "Redis",
                failureStatus: HealthStatus.Unhealthy,
                tags: new[] {"Cashe", "Queue"});
        
        services.AddHttpClient();
        services
            .AddHealthChecksUI(options =>
            {
                options.SetApiMaxActiveRequests(1);
                options.SetEvaluationTimeInSeconds(60);
                options.AddHealthCheckEndpoint("Healthcheck API", HealthCheckEndpointName);
            })
            .AddInMemoryStorage();        
        return services;
    }

    public static IApplicationBuilder UseHealthCheckConfiguration(this WebApplication app)
    {
        app.MapHealthChecks(HealthCheckEndpointName, new()
        {
            ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
        });
        app.MapHealthChecksUI(options => options.UIPath = HealthCheckEndpointDashboardName);
        return app;
    }
}