using Application.DTO.Enums;

namespace Application.Repositories;

public interface IRideStatusRepository
{
    Task<RideStatus> GetRideStatus(long rideId, CancellationToken token);

    Task ChangeRideStatus(long rideId, RideStatus toRideStatus, CancellationToken token);
}