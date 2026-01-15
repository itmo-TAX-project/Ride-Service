using Application.Ports;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Presentation.Grpc.ClientServices;
using Routes.Client.Contracts;

namespace Presentation.Extensions;

public static class GrpcExtensions
{
    public static IServiceCollection AddGrpcClients(this IServiceCollection services, IConfiguration configuration)
    {
        string routeServiceAddress =
            configuration.GetValue<string>("Grpc:RouteServiceAddress")
            ?? throw new InvalidOperationException("Grpc:RouteServiceAddress is not configured");

        services.AddGrpcClient<RouteService.RouteServiceClient>(options =>
        {
            options.Address = new Uri(routeServiceAddress);
        });
        services.AddScoped<IRouteServicePort, RouteServiceGrpcClient>();

        return services;
    }
}