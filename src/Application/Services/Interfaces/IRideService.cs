using Application.DTO;

namespace Application.Services.Interfaces;

public interface IRideService
{
    Task<long> CreateRideAsync(
        long passengerId,
        PointDto pickupLocation,
        PointDto dropOffLocation,
        CancellationToken cancellationToken);

    Task<RideDto?> GetPersonCurrentRideAsync(long passengerId, CancellationToken cancellationToken);

    Task<RideDto?> GetRideAsync(long rideId, CancellationToken cancellationToken);
}