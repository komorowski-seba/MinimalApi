using Application.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Application.Job;

public static class BgServiceExtension
{
    public static IServiceCollection AddBgServices(this IServiceCollection services)
    {
        services.AddSingleton<IFindEventService, FindEventService>();
        services.AddHostedService<BgDomainEventsService>();
        return services;
    }
}