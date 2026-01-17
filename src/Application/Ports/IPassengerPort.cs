namespace Application.Ports;

public interface IPassengerPort
{
    Task<long> GetPassengerAccountIdAsync(long passengerId, CancellationToken cancellationToken);
}