using Application.Ports.ProducersPorts;
using Itmo.Dev.Platform.Kafka.Configuration;
using Itmo.Dev.Platform.Kafka.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Presentation.Kafka.Consumers;
using Presentation.Kafka.Consumers.Keys;
using Presentation.Kafka.Consumers.Values;
using Presentation.Kafka.Producers;
using Presentation.Kafka.Producers.Keys;
using Presentation.Kafka.Producers.Values;

namespace Presentation.Extensions;

public static class KafkaExtensions
{
    public static IServiceCollection AddKafka(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // https://github.com/itmo-is-dev/platform/wiki/Kafka:-Configuration
        services.AddPlatformKafka(builder => builder
            .ConfigureOptions(configuration.GetSection("Kafka"))

            .AddRideAssignationConsumer(configuration)

            .AddRideAssignedProducer(configuration)
            .AddRideCancelledProducer(configuration)
            .AddRideCompletedProducer(configuration)
            .AddRideRequestedProducer(configuration)
            .AddRideStartedProducer(configuration));

        services.AddScoped<IRideProducer, RideProducer>();
        return services;
    }

    private static IKafkaConfigurationBuilder AddRideAssignedProducer(
        this IKafkaConfigurationBuilder builder,
        IConfiguration configuration)
    {
        return builder.AddProducer(p => p
            .WithKey<RideProcessorKey>()
            .WithValue<RideConfirmedValue>()
            .WithConfiguration(configuration.GetSection("Kafka:Producers:RideAssignedMessage"))
            .SerializeKeyWithNewtonsoft()
            .SerializeValueWithNewtonsoft()
            .WithOutbox());
    }

    private static IKafkaConfigurationBuilder AddRideCancelledProducer(
        this IKafkaConfigurationBuilder builder,
        IConfiguration configuration)
    {
        return builder.AddProducer(p => p
            .WithKey<RideProcessorKey>()
            .WithValue<RideCancelledValue>()
            .WithConfiguration(configuration.GetSection("Kafka:Producers:RideCancelledMessage"))
            .SerializeKeyWithNewtonsoft()
            .SerializeValueWithNewtonsoft()
            .WithOutbox());
    }

    private static IKafkaConfigurationBuilder AddRideCompletedProducer(
        this IKafkaConfigurationBuilder builder,
        IConfiguration configuration)
    {
        IConfigurationSection section = configuration.GetSection("Kafka:Producers:RideCompletedMessage");
        return builder.AddProducer(p => p
            .WithKey<RideProcessorKey>()
            .WithValue<RideCompletedValue>()
            .WithConfiguration(configuration.GetSection("Kafka:Producers:RideCompletedMessage"))
            .SerializeKeyWithNewtonsoft()
            .SerializeValueWithNewtonsoft()
            .WithOutbox());
    }

    private static IKafkaConfigurationBuilder AddRideRequestedProducer(
        this IKafkaConfigurationBuilder builder,
        IConfiguration configuration)
    {
        return builder.AddProducer(p => p
            .WithKey<RideProcessorKey>()
            .WithValue<RideRequestedValue>()
            .WithConfiguration(configuration.GetSection("Kafka:Producers:RideRequestedMessage"))
            .SerializeKeyWithNewtonsoft()
            .SerializeValueWithNewtonsoft()
            .WithOutbox());
    }

    private static IKafkaConfigurationBuilder AddRideStartedProducer(
        this IKafkaConfigurationBuilder builder,
        IConfiguration configuration)
    {
        return builder.AddProducer(p => p
            .WithKey<RideProcessorKey>()
            .WithValue<RideStartedValue>()
            .WithConfiguration(configuration.GetSection("Kafka:Producers:RideStartedMessage"))
            .SerializeKeyWithNewtonsoft()
            .SerializeValueWithNewtonsoft()
            .WithOutbox());
    }

    private static IKafkaConfigurationBuilder AddRideAssignationConsumer(
        this IKafkaConfigurationBuilder builder,
        IConfiguration configuration)
    {
        return builder.AddConsumer(c => c
            .WithKey<RideKey>()
            .WithValue<RideAssignationMessage>()
            .WithConfiguration(configuration.GetSection("Kafka:Consumers:RideAssignationMessage"))
            .DeserializeKeyWithNewtonsoft()
            .DeserializeValueWithNewtonsoft()
            .HandleInboxWith<DispatchDriverAssignation>());
    }
}