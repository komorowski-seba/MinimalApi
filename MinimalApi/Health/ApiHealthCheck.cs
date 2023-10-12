using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace MinimalApi.Health;

public sealed class ApiHealthCheck : IHealthCheck 
{
    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = new CancellationToken())
    {
        try
        {
            return await Task
                .FromResult(HealthCheckResult.Healthy("The service is up and running"));
        }
        catch (Exception)
        {
            return await Task
                .FromResult(new HealthCheckResult(context.Registration.FailureStatus, "The service is down"));
        }
    }
}