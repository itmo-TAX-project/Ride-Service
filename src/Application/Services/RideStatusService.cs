using Application.DTO.Enums;
using Application.Repositories;
using Application.Services.Interfaces;
using System.Transactions;

namespace Application.Services;

public class RideStatusService : IRideStatusService
{
    private readonly IRideStatusRepository _rideStatusRepository;

    public RideStatusService(IRideStatusRepository rideStatusRepository)
    {
        _rideStatusRepository = rideStatusRepository;
    }

    public bool CanChange(RideStatus fromRideStatus, RideStatus toRideStatus)
    {
        return fromRideStatus switch
        {
            RideStatus.Requested => toRideStatus is RideStatus.SearchingDriver or RideStatus.Cancelled,
            RideStatus.SearchingDriver => toRideStatus is RideStatus.DriverAssigned or RideStatus.Cancelled,
            RideStatus.DriverAssigned => toRideStatus is RideStatus.Confirmed or RideStatus.Cancelled,
            RideStatus.Confirmed => toRideStatus is RideStatus.Started or RideStatus.Cancelled,
            RideStatus.Started => toRideStatus is RideStatus.Completed,
            RideStatus.Completed => false,
            RideStatus.Cancelled => false,
            _ => false,
        };
    }

    public async Task<RideStatus> GetRideStatus(long rideId, CancellationToken cancellationToken)
    {
        return await _rideStatusRepository.GetRideStatus(rideId, cancellationToken);
    }

    public async Task ChangeRideStatus(long rideId, RideStatus toRideStatus, CancellationToken cancellationToken)
    {
        TransactionScope transaction = CreateTransactionScope();

        RideStatus fromRideStatus = await GetRideStatus(rideId, cancellationToken);
        if (!CanChange(fromRideStatus, toRideStatus))
        {
            throw new InvalidOperationException("Cannot change the status of a ride because if is invalid");
        }

        await _rideStatusRepository.ChangeRideStatus(rideId, toRideStatus, cancellationToken);

        transaction.Complete();
        transaction.Dispose();
    }

    private static TransactionScope CreateTransactionScope()
    {
        return new TransactionScope(
            TransactionScopeOption.Required,
            new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted },
            TransactionScopeAsyncFlowOption.Enabled);
    }
}