using Application.Ports.ProducersPorts;
using Application.Ports.ProducersPorts.Events;
using Itmo.Dev.Platform.Kafka.Extensions;
using Itmo.Dev.Platform.Kafka.Producer;
using Presentation.Kafka.Producers.Keys;
using Presentation.Kafka.Producers.Values;

namespace Presentation.Kafka.Producers;

public class RideProducer : IRideProducer
{
    private readonly IKafkaMessageProducer<RideProcessorKey, RideRequestedValue> _rideRequestedProducer;
    private readonly IKafkaMessageProducer<RideProcessorKey, RideStartedValue> _rideStartedProducer;
    private readonly IKafkaMessageProducer<RideProcessorKey, RideCompletedValue> _rideCompletedProducer;
    private readonly IKafkaMessageProducer<RideProcessorKey, RideCancelledValue> _rideCancelledProducer;
    private readonly IKafkaMessageProducer<RideProcessorKey, RideAssignedValue> _rideAssignedProducer;

    public RideProducer(
        IKafkaMessageProducer<RideProcessorKey, RideRequestedValue> rideRequestedProducer,
        IKafkaMessageProducer<RideProcessorKey, RideStartedValue> rideStartedProducer,
        IKafkaMessageProducer<RideProcessorKey, RideCompletedValue> rideCompletedProducer,
        IKafkaMessageProducer<RideProcessorKey, RideCancelledValue> rideCancelledProducer,
        IKafkaMessageProducer<RideProcessorKey, RideAssignedValue> rideAssignedProducer)
    {
        _rideRequestedProducer = rideRequestedProducer;
        _rideStartedProducer = rideStartedProducer;
        _rideCompletedProducer = rideCompletedProducer;
        _rideCancelledProducer = rideCancelledProducer;
        _rideAssignedProducer = rideAssignedProducer;
    }

    public async Task ProduceAsync(RideRequestedEvent rideRequestedEvent, CancellationToken cancellationToken)
    {
        var key = new RideProcessorKey() { RideId = rideRequestedEvent.RideId };
        var value = new RideRequestedValue()
        {
            RideId = rideRequestedEvent.RideId,
            PassengerId = rideRequestedEvent.PassengerId,
            PickupLocation = rideRequestedEvent.PickupLocation,
            DropLocation = rideRequestedEvent.DropLocation,
        };
        var message = new KafkaProducerMessage<RideProcessorKey, RideRequestedValue>(key, value);

        await _rideRequestedProducer.ProduceAsync(message, cancellationToken);
    }

    public async Task ProduceAsync(RideStartedEvent rideStartedEvent, CancellationToken cancellationToken)
    {
        var key = new RideProcessorKey() { RideId = rideStartedEvent.RideId };
        var value = new RideStartedValue() { RideId = rideStartedEvent.RideId };
        var message = new KafkaProducerMessage<RideProcessorKey, RideStartedValue>(key, value);

        await _rideStartedProducer.ProduceAsync(message, cancellationToken);
    }

    public async Task ProduceAsync(RideCancelledEvent rideCancelledEvent, CancellationToken cancellationToken)
    {
        var key = new RideProcessorKey() { RideId = rideCancelledEvent.RideId };
        var value = new RideCancelledValue() { RideId = rideCancelledEvent.RideId };
        var message = new KafkaProducerMessage<RideProcessorKey, RideCancelledValue>(key, value);

        await _rideCancelledProducer.ProduceAsync(message, cancellationToken);
    }

    public async Task ProduceAsync(RideCompletedEvent rideCompletedEvent, CancellationToken cancellationToken)
    {
        var key = new RideProcessorKey() { RideId = rideCompletedEvent.RideId };
        var value = new RideCompletedValue() { RideId = rideCompletedEvent.RideId };
        var message = new KafkaProducerMessage<RideProcessorKey, RideCompletedValue>(key, value);

        await _rideCompletedProducer.ProduceAsync(message, cancellationToken);
    }

    public async Task ProduceAsync(RideAssignedEvent rideCancelledEvent, CancellationToken cancellationToken)
    {
        var key = new RideProcessorKey() { RideId = rideCancelledEvent.RideId };
        var value = new RideAssignedValue { RideId = rideCancelledEvent.RideId, DriverId = rideCancelledEvent.DriverId };
        var message = new KafkaProducerMessage<RideProcessorKey, RideAssignedValue>(key, value);

        await _rideAssignedProducer.ProduceAsync(message, cancellationToken);
    }
}