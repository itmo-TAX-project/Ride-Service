using Application.DTO.Enums;

namespace Application.DTO;

public class DriverStatusDto
{
    public long DriverId { get; set; }

    public required PointDto Location { get; set; }

    public DriverAvailability Availability { get; set; }

    public DateTime Timestamp { get; set; }
}