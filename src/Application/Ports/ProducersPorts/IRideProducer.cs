using Application.Ports.ProducersPorts.Events;

namespace Application.Ports.ProducersPorts;

public interface IRideProducer
{
    Task ProduceAsync(RideRequestedEvent rideRequestedEvent, CancellationToken cancellationToken);

    Task ProduceAsync(RideCancelledEvent rideCancelledEvent, CancellationToken cancellationToken);

    Task ProduceAsync(RideStartedEvent rideStartedEvent, CancellationToken cancellationToken);

    Task ProduceAsync(RideCompletedEvent rideCompletedEvent, CancellationToken cancellationToken);

    Task ProduceAsync(RideAssignedEvent rideCancelledEvent, CancellationToken cancellationToken);
}