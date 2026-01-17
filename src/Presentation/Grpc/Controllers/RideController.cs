using Application.DTO;
using Application.Services.Interfaces;
using Grpc.Core;
using Presentation.Grpc.Mapping;
using Rides.RideService.Contracts;

namespace Presentation.Grpc.Controllers;

public class RideController : RideService.RideServiceBase
{
    private readonly IRideService _rideService;

    private readonly IRideLogisticService _rideLogisticService;

    public RideController(IRideService rideService, IRideLogisticService rideLogisticService)
    {
        _rideService = rideService;
        _rideLogisticService = rideLogisticService;
    }

    public override async Task<CreateRideResponse> CreateRide(CreateRideRequest request, ServerCallContext context)
    {
        long rideId = await _rideService.CreateRideAsync(
            request.PassengerId,
            new PointDto(request.PickupLatitude, request.PickupLongitude),
            new PointDto(request.DropoffLatitude, request.DropoffLongitude),
            context.CancellationToken);

        return new CreateRideResponse { RideId = rideId };
    }

    public override async Task<GetRideResponse> GetRide(GetRideRequest request, ServerCallContext context)
    {
        Application.DTO.RideDto? rideDto = await _rideService.GetRideAsync(request.RideId, context.CancellationToken);

        return new GetRideResponse
        {
            Dto = rideDto.ToRpc(),
        };
    }

    public override async Task<CancelRideResponse> CancelRide(CancelRideRequest request, ServerCallContext context)
    {
        await _rideLogisticService.CancelRideAsync(request.RideId, context.CancellationToken);

        return new CancelRideResponse();
    }
}