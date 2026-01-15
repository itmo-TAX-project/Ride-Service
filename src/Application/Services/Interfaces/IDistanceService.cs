using Application.DTO;

namespace Application.Services.Interfaces;

public interface IDistanceService
{
    bool IsNearPickUp(PointDto ridePickUpLocation, PointDto driverPosition, CancellationToken token);

    bool IsNearDropoff(PointDto rideDropOffLocation, PointDto driverPosition, CancellationToken token);
}