using Application.Services.Interfaces;
using Itmo.Dev.Platform.Kafka.Consumer;
using Presentation.Kafka.Consumers.Keys;
using Presentation.Kafka.Consumers.Values;

namespace Presentation.Kafka.Consumers;

public class ConfirmRideHandler : IKafkaInboxHandler<RideKey, ConfirmRideValue>
{
    private readonly IRideLogisticService _rideLogisticService;

    public ConfirmRideHandler(IRideLogisticService rideLogisticService)
    {
        _rideLogisticService = rideLogisticService;
    }

    public async ValueTask HandleAsync(
        IEnumerable<IKafkaInboxMessage<RideKey, ConfirmRideValue>> messages,
        CancellationToken cancellationToken)
    {
        foreach (IKafkaInboxMessage<RideKey, ConfirmRideValue> message in messages)
        {
            await _rideLogisticService.RideConfirmedAsync(
                message.Value.RideId,
                message.Value.DriverId,
                cancellationToken);
        }
    }
}