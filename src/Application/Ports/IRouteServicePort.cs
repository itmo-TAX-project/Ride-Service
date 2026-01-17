using Application.DTO;

namespace Application.Ports;

public interface IRouteServicePort
{
    Task<long> CalculateRouteAsync(
        PointDto pickup,
        PointDto dropoff,
        CancellationToken cancellationToken);

    Task<RouteMetadataDto> GetRouteMetadataAsync(long routeId, CancellationToken cancellationToken);
}