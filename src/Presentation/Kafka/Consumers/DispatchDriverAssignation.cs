using Application.Services.Interfaces;
using Itmo.Dev.Platform.Kafka.Consumer;
using Presentation.Kafka.Consumers.Keys;
using Presentation.Kafka.Consumers.Values;

namespace Presentation.Kafka.Consumers;

public class DispatchDriverAssignation : IKafkaInboxHandler<RideKey, RideAssignationMessage>
{
    private readonly IRideLogisticService _rideLogisticService;

    public DispatchDriverAssignation(IRideLogisticService rideLogisticService)
    {
        _rideLogisticService = rideLogisticService;
    }

    public async ValueTask HandleAsync(
        IEnumerable<IKafkaInboxMessage<RideKey, RideAssignationMessage>> messages,
        CancellationToken cancellationToken)
    {
        foreach (IKafkaInboxMessage<RideKey, RideAssignationMessage> message in messages)
        {
            RideAssignationMessage value = message.Value;
            if (value.AssignationStatus == AssignationStatus.Failed)
            {
                await _rideLogisticService.DriverAssignationFailed(message.Key.RideId, cancellationToken);
            }
            else
            {
                await _rideLogisticService.DriverAssigned(message.Key.RideId, message.Value.DriverId, cancellationToken);
            }
        }
    }
}