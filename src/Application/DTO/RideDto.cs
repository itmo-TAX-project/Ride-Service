using Application.DTO.Enums;

namespace Application.DTO;

public class RideDto
{
    public long RideId { get; set; }

    public long PassengerId { get; set; }

    public required PointDto PickupLocation { get; set; }

    public required PointDto DropLocation { get; set; }

    public RideStatus Status { get; set; }

    public long? AssignedDriverId { get; set; }

    public long? RouteId { get; set; }
}