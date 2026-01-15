using Application.DTO;

namespace Application.Ports.ProducersPorts.Events;

public class RideRequestedEvent : IEventMessage
{
    public long RideId { get; set; }

    public long PassengerId { get; set; }

    public required PointDto PickupLocation { get; set; }

    public required PointDto DropLocation { get; set; }
}