namespace Application.Ports.ProducersPorts.Events;

public class RideCancelledEvent : IEventMessage
{
    public long RideId { get; set; }
}