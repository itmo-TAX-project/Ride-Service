using Application.DTO;

namespace Application.Repositories;

public interface IRideRepository
{
    Task<long> CreateRideAsync(RideDto dto, CancellationToken cancellationToken);

    Task<RideDto?> GetRideAsync(long rideId, CancellationToken cancellationToken);

    Task AddRideDriverAsync(long rideId, long driverId, CancellationToken cancellationToken);

    Task AddRouteAsync(long rideId, long routeId, CancellationToken cancellationToken);

    Task<RideDto?> GetActiveRideByPassengerIdAsync(long passengerId, CancellationToken cancellationToken);

    Task<RideDto?> GetActiveRideByDriverIdAsync(long driverId, CancellationToken cancellationToken);
}