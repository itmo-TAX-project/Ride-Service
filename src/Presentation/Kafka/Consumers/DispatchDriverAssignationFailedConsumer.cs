using Application.Services.Interfaces;
using Itmo.Dev.Platform.Kafka.Consumer;
using Presentation.Kafka.Consumers.Keys;
using Presentation.Kafka.Consumers.Values;

namespace Presentation.Kafka.Consumers;

public class DispatchDriverAssignationFailedConsumer : IKafkaInboxHandler<RideKey, RideAssignationFailed>
{
    private readonly IRideLogisticService _rideLogisticService;

    public DispatchDriverAssignationFailedConsumer(IRideLogisticService rideLogisticService)
    {
        _rideLogisticService = rideLogisticService;
    }

    public async ValueTask HandleAsync(
        IEnumerable<IKafkaInboxMessage<RideKey, RideAssignationFailed>> messages,
        CancellationToken cancellationToken)
    {
        foreach (IKafkaInboxMessage<RideKey, RideAssignationFailed> message in messages)
        {
            await _rideLogisticService.DriverAssignationFailed(message.Key.RideId, cancellationToken);
        }
    }
}