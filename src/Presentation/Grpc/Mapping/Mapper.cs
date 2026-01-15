using Application.DTO;
using Application.DTO.Enums;

namespace Presentation.Grpc.Mapping;

public static class Mapper
{
    public static Rides.RideService.Contracts.RideStatus ToRpc(this RideStatus status)
    {
        return status switch
        {
            RideStatus.Requested => Rides.RideService.Contracts.RideStatus.Requested,
            RideStatus.SearchingDriver => Rides.RideService.Contracts.RideStatus.Searching,
            RideStatus.DriverAssigned => Rides.RideService.Contracts.RideStatus.Assigned,
            RideStatus.Confirmed => Rides.RideService.Contracts.RideStatus.Confirmed,
            RideStatus.Started => Rides.RideService.Contracts.RideStatus.Started,
            RideStatus.Completed => Rides.RideService.Contracts.RideStatus.Completed,
            RideStatus.Cancelled => Rides.RideService.Contracts.RideStatus.Cancelled,
            _ => throw new ArgumentOutOfRangeException(nameof(status), status, null),
        };
    }

    public static Rides.RideService.Contracts.RideDto? ToRpc(this RideDto? dto)
    {
        if (dto == null) return null;

        return new Rides.RideService.Contracts.RideDto()
        {
            RideId = dto.RideId,
            PassengerId = dto.PassengerId,
            AssignedDriverId = dto.AssignedDriverId,
            PickupLatitude = dto.PickupLocation.Latitude,
            PickupLongitude = dto.PickupLocation.Longitude,
            DropoffLatitude = dto.DropLocation.Latitude,
            DropoffLongitude = dto.DropLocation.Longitude,
            Status = dto.Status.ToRpc(),
        };
    }
}