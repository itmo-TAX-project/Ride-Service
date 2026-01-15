using Application.DTO;

namespace Application.Ports;

public interface IRouteServicePort
{
    Task<long> CalculateRoute(
        PointDto pickup,
        PointDto dropoff,
        CancellationToken cancellationToken);
}