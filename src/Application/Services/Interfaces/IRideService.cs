using Application.DTO;

namespace Application.Services.Interfaces;

public interface IRideService
{
    Task<long> CreateRide(
        long passengerId,
        PointDto pickupLocation,
        PointDto dropOffLocation,
        CancellationToken cancellationToken);

    Task<RideDto?> GetPersonCurrentRide(long passengerId, CancellationToken cancellationToken);

    Task<RideDto?> GetRide(long rideId, CancellationToken cancellationToken);
}