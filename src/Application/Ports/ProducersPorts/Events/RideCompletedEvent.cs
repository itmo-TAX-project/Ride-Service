namespace Application.Ports.ProducersPorts.Events;

public class RideCompletedEvent : IEventMessage
{
    public long RideId { get; set; }
}