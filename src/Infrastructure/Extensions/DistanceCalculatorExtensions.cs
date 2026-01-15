using Application.Ports;
using Infrastructure.DistanceCalculator;
using Infrastructure.DistanceCalculator.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Extensions;

public static class DistanceCalculatorExtensions
{
    public static IServiceCollection AddDistanceCalculator(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<DistanceOptions>(configuration.GetSection("DistanceOptions"));
        return services.AddScoped<IDistanceCalculator, CalculateDistanceService>();
    }
}