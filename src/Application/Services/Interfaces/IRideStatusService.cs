using Application.DTO.Enums;

namespace Application.Services.Interfaces;

public interface IRideStatusService
{
    bool CanChange(RideStatus fromRideStatus, RideStatus toRideStatus);

    Task<RideStatus> GetRideStatus(long rideId, CancellationToken cancellationToken);

    Task ChangeRideStatus(long rideId, RideStatus toRideStatus, CancellationToken cancellationToken);
}