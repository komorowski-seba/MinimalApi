using System.Reflection;
using Application.Cache;
using Application.Job;
using Application.Services;
using Domain.Common.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Application;

public static class ApplicationExtension
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMediatR(cfg=>cfg.RegisterServicesFromAssemblies(Assembly.GetExecutingAssembly()));
        services.AddBgServices();
        services.AddCacheServices(configuration);
        services.AddScoped<IGuidAndTimeProvider, GuidAndTimeProviderService>();
        return services;
    }
}