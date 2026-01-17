using Application.DTO.Enums;

namespace Presentation.Kafka.Consumers.Values;

public class TaxiDriverStatusChangedMessage
{
    public long DriverId { get; init; }

    public double Latitude { get; init; }

    public double Longitude { get; init; }

    public DriverAvailability Availability { get; init; }

    public DateTime Timestamp { get; init; }
}