using Application.DTO.Enums;

namespace Application.Repositories;

public interface IRideStatusRepository
{
    Task<RideStatus> GetRideStatusAsync(long rideId, CancellationToken token);

    Task ChangeRideStatusAsync(long rideId, RideStatus toRideStatus, CancellationToken token);
}