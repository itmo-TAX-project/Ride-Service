using Application.Services.Interfaces;
using Itmo.Dev.Platform.Kafka.Consumer;
using Presentation.Kafka.Consumers.Keys;
using Presentation.Kafka.Consumers.Values;

namespace Presentation.Kafka.Consumers;

public class DispatchDriverAssignedConsumer : IKafkaInboxHandler<RideKey, RideAssignedConsumerValue>
{
    private readonly IRideLogisticService _rideLogisticService;

    public DispatchDriverAssignedConsumer(IRideLogisticService rideLogisticService)
    {
        _rideLogisticService = rideLogisticService;
    }

    public async ValueTask HandleAsync(
        IEnumerable<IKafkaInboxMessage<RideKey, RideAssignedConsumerValue>> messages,
        CancellationToken cancellationToken)
    {
        foreach (IKafkaInboxMessage<RideKey, RideAssignedConsumerValue> message in messages)
        {
            await _rideLogisticService.DriverAssigned(message.Key.RideId, message.Value.DriverId, cancellationToken);
        }
    }
}