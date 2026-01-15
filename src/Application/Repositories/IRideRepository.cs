using Application.DTO;

namespace Application.Repositories;

public interface IRideRepository
{
    Task<long> CreateRide(RideDto dto, CancellationToken cancellationToken);

    Task<RideDto?> GetRide(long rideId, CancellationToken cancellationToken);

    Task AddRideDriver(long rideId, long driverId, CancellationToken cancellationToken);

    Task AddRoute(long rideId, long routeId, CancellationToken cancellationToken);

    Task<RideDto?> GetActiveRideByPassengerId(long passengerId, CancellationToken cancellationToken);

    Task<RideDto?> GetActiveRideByDriverId(long driverId, CancellationToken cancellationToken);
}