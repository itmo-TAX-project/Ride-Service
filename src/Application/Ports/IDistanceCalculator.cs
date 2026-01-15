using Application.DTO;

namespace Application.Ports;

public interface IDistanceCalculator
{
    double CalculateMeters(PointDto a, PointDto b);
}