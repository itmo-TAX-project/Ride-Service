namespace Presentation.Kafka.Producers.Values;

public class RideCompletedValue
{
    public long AccountId { get; set; }

    public long RideId { get; set; }

    public decimal DurationMeters { get; set; }

    public long DurationTime { get; set; }
}