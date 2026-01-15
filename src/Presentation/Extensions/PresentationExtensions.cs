using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Presentation.Grpc.Interceptors;

namespace Presentation.Extensions;

public static class PresentationExtensions
{
    public static IServiceCollection AddPresentation(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<ErrorHandlerInterceptor>();
        services.AddGrpc(options => options.Interceptors.Add<ErrorHandlerInterceptor>());
        services.AddGrpcClients(configuration);
        services.AddGrpcReflection();
        services.AddKafka(configuration);

        return services;
    }
}