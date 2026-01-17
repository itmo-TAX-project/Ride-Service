namespace Application.Ports.ProducersPorts.Events;

public class RideCompletedEvent : IEventMessage
{
    public long AccountId { get; set; }

    public long RideId { get; set; }

    public decimal DurationMeters { get; set; }

    public long DurationTime { get; set; }
}