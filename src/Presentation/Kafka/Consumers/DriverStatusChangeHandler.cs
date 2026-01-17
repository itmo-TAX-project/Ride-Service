using Application.DTO;
using Application.Services.Interfaces;
using Itmo.Dev.Platform.Kafka.Consumer;
using Presentation.Kafka.Consumers.Keys;
using Presentation.Kafka.Consumers.Values;

namespace Presentation.Kafka.Consumers;

public class DriverStatusChangeHandler : IKafkaInboxHandler<TaxiDriverStatusChangedMessageKey, TaxiDriverStatusChangedMessage>
{
    private readonly IRideLogisticService _rideLogisticService;

    public DriverStatusChangeHandler(IRideLogisticService rideLogisticService)
    {
        _rideLogisticService = rideLogisticService;
    }

    public async ValueTask HandleAsync(
        IEnumerable<IKafkaInboxMessage<TaxiDriverStatusChangedMessageKey, TaxiDriverStatusChangedMessage>> messages,
        CancellationToken cancellationToken)
    {
        foreach (IKafkaInboxMessage<TaxiDriverStatusChangedMessageKey, TaxiDriverStatusChangedMessage> message in messages)
        {
            TaxiDriverStatusChangedMessage value = message.Value;
            var driverStatusDto = new DriverStatusDto()
            {
                DriverId = value.DriverId,
                Location = new PointDto(value.Latitude, value.Longitude),
                Availability = value.Availability,
                Timestamp = value.Timestamp,
            };

            await _rideLogisticService.DriverPositionChangedAsync(driverStatusDto, cancellationToken);
        }
    }
}