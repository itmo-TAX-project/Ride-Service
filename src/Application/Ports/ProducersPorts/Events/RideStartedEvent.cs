namespace Application.Ports.ProducersPorts.Events;

public class RideStartedEvent : IEventMessage
{
    public long RideId { get; set; }
}