namespace Application.Ports.ProducersPorts.Events;

public class RideConfirmedEvent : IEventMessage
{
    public long RideId { get; set; }

    public long DriverId { get; set; }
}