using Application.Options;
using Application.Services;
using Application.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Application.Extensions;

public static class ApplicationExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IDistanceService, DistanceService>();
        services.AddScoped<IRideLogisticService, RideLogisticService>();
        services.AddScoped<IRideStatusService, RideStatusService>();
        services.AddScoped<IRideService, RideService>();

        services.Configure<RidePointsOptions>(configuration.GetSection("RidePoints"));
        return services;
    }
}