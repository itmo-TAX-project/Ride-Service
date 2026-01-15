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

    public async Task<long> CalculateRoute(PointDto pickup, PointDto dropoff, CancellationToken cancellationToken)
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
}