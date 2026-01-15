using Application.DTO;

namespace Presentation.Kafka.Producers.Values;

public class RideRequestedValue
{
    public long RideId { get; set; }

    public long PassengerId { get; set; }

    public required PointDto PickupLocation { get; set; }

    public required PointDto DropLocation { get; set; }
}