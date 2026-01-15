using Application.DTO;
using Application.Options;
using Application.Ports;
using Application.Services.Interfaces;
using Microsoft.Extensions.Options;

namespace Application.Services;

public class DistanceService : IDistanceService
{
    private readonly RidePointsOptions _options;
    private readonly IDistanceCalculator _distanceCalculator;

    public DistanceService(IOptions<RidePointsOptions> options, IDistanceCalculator distanceCalculator)
    {
        _distanceCalculator = distanceCalculator;
        _options = options.Value;
    }

    public bool IsNearPickUp(PointDto ridePickUpLocation, PointDto driverPosition, CancellationToken token)
    {
        double distance = _distanceCalculator.CalculateMeters(ridePickUpLocation, driverPosition);
        return distance <= _options.PickupArrivalMeters;
    }

    public bool IsNearDropoff(PointDto rideDropOffLocation, PointDto driverPosition, CancellationToken token)
    {
        double distance = _distanceCalculator.CalculateMeters(rideDropOffLocation, driverPosition);
        return distance <= _options.DropoffArrivalMeters;
    }
}