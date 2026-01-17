using Application.DTO;
using Application.Ports;
using Routes.Client.Contracts;

namespace Presentation.Grpc.ClientServices;

public class RouteServiceGrpcClient : IRouteServicePort
{
    private readonly RouteService.RouteServiceClient _grpcClient;

    public RouteServiceGrpcClient(RouteService.RouteServiceClient grpcClient)
    {
        _grpcClient = grpcClient;
    }

    public async Task<long> CalculateRouteAsync(PointDto pickup, PointDto dropoff, CancellationToken cancellationToken)
    {
        var request = new CalculateRouteRequest
        {
            Pickup = new Point() { Latitude = pickup.Latitude, Longitude = pickup.Longitude },
            Dropoff = new Point() { Latitude = dropoff.Latitude, Longitude = dropoff.Longitude },
        };

        CalculateRouteResponse response = await _grpcClient.CalculateRouteAsync(
            request,
            cancellationToken: cancellationToken);
        return response.RouteId;
    }

    public async Task<RouteMetadataDto> GetRouteMetadataAsync(long routeId, CancellationToken cancellationToken)
    {
        var request = new GetRouteRequest
        {
            RouteId = routeId,
        };

        GetRouteResponse response = await _grpcClient.GetRouteAsync(
            request,
            cancellationToken: cancellationToken);
        return new RouteMetadataDto
        {
            DurationMeters = (decimal)response.Route.DistanceM,
            DurationTime = response.Route.DurationS,
        };
    }
}