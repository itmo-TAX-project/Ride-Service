using Application.DTO;

namespace Application.Services.Interfaces;

public interface IRideLogisticService
{
    Task DriverAssigned(long rideId, long driverId, CancellationToken cancellationToken);

    Task DriverAssignationFailed(long rideId, CancellationToken cancellationToken);

    Task RideConfirmed(long rideId, long driverId, CancellationToken cancellationToken);

    Task CancelRide(long rideId, CancellationToken cancellationToken);

    Task DriverPositionChanged(DriverStatusDto driverDto, CancellationToken cancellationToken);
}