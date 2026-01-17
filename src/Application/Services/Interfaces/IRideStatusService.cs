using Application.DTO.Enums;

namespace Application.Services.Interfaces;

public interface IRideStatusService
{
    bool CanChange(RideStatus fromRideStatus, RideStatus toRideStatus);

    Task<RideStatus> GetRideStatusAsync(long rideId, CancellationToken cancellationToken);

    Task ChangeRideStatusAsync(long rideId, RideStatus toRideStatus, CancellationToken cancellationToken);
}