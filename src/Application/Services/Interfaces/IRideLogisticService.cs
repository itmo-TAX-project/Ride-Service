using Application.DTO;

namespace Application.Services.Interfaces;

public interface IRideLogisticService
{
    Task DriverAssignedAsync(long rideId, long driverId, CancellationToken cancellationToken);

    Task DriverAssignationFailedAsync(long rideId, CancellationToken cancellationToken);

    Task RideConfirmedAsync(long rideId, long driverId, CancellationToken cancellationToken);

    Task CancelRideAsync(long rideId, CancellationToken cancellationToken);

    Task DriverPositionChangedAsync(DriverStatusDto driverDto, CancellationToken cancellationToken);
}