namespace Application.Ports.ProducersPorts.Events;

public class RideAssignedEvent : IEventMessage
{
    public long RideId { get; set; }

    public long DriverId { get; set; }
}