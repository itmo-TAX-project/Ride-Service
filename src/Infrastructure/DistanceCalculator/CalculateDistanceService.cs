using Application.DTO;
using Application.Ports;
using Infrastructure.DistanceCalculator.Options;
using Microsoft.Extensions.Options;

namespace Infrastructure.DistanceCalculator;

public class CalculateDistanceService : IDistanceCalculator
{
    private readonly DistanceOptions _options;

    public CalculateDistanceService(IOptions<DistanceOptions> options)
    {
        _options = options.Value;
    }

    public double CalculateMeters(PointDto a, PointDto b)
    {
        double lat1 = DegreesToRadians(a.Latitude);
        double lon1 = DegreesToRadians(a.Longitude);
        double lat2 = DegreesToRadians(b.Latitude);
        double lon2 = DegreesToRadians(b.Longitude);

        double deltaLat = lat2 - lat1;
        double deltaLon = lon2 - lon1;

        double sinLat = Math.Sin(deltaLat / 2);
        double sinLon = Math.Sin(deltaLon / 2);

        double h = (sinLat * sinLat) + (Math.Cos(lat1) * Math.Cos(lat2) * sinLon * sinLon);

        double c = 2 * Math.Asin(Math.Min(1, Math.Sqrt(h)));

        return _options.EarthRadiusMeters * c;
    }

    private static double DegreesToRadians(double degrees)
    {
        return degrees * Math.PI / 180;
    }
}