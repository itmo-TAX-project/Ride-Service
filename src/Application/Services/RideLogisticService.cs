using Application.DTO;
using Application.DTO.Enums;
using Application.Ports.ProducersPorts;
using Application.Ports.ProducersPorts.Events;
using Application.Repositories;
using Application.Services.Interfaces;
using System.Transactions;

namespace Application.Services;

public class RideLogisticService : IRideLogisticService
{
    private readonly IRideRepository _rideRepository;

    private readonly IRideStatusService _rideStatusService;

    private readonly IDistanceService _distanceService;

    private readonly IRideProducer _rideProducer;

    public RideLogisticService(
        IRideRepository rideRepository,
        IRideStatusService rideStatusService,
        IDistanceService distanceService,
        IRideProducer rideProducer)
    {
        _rideRepository = rideRepository;
        _rideStatusService = rideStatusService;
        _distanceService = distanceService;
        _rideProducer = rideProducer;
    }

    public async Task DriverAssigned(long rideId, long driverId, CancellationToken cancellationToken)
    {
        TransactionScope transaction = CreateTransactionScope();

        if (!await CheckIfRideExistsAsync(rideId, cancellationToken))
        {
            throw new Exception("Ride doesn't exist");
        }

        await _rideRepository.AddRideDriver(rideId, driverId, cancellationToken);
        await _rideStatusService.ChangeRideStatus(rideId, RideStatus.DriverAssigned, cancellationToken);

        var rideAssignedMessage = new RideAssignedEvent()
        {
            RideId = rideId,
            DriverId = driverId,
        };
        await _rideProducer.ProduceAsync(rideAssignedMessage, cancellationToken);

        transaction.Complete();
        transaction.Dispose();
    }

    public async Task DriverAssignationFailed(long rideId, CancellationToken cancellationToken)
    {
        TransactionScope transaction = CreateTransactionScope();

        if (!await CheckIfRideExistsAsync(rideId, cancellationToken))
        {
            throw new Exception("Ride doesn't exist");
        }

        await _rideStatusService.ChangeRideStatus(rideId, RideStatus.Cancelled, cancellationToken);

        transaction.Complete();
        transaction.Dispose();
    }

    public async Task RideConfirmed(long rideId, long driverId, CancellationToken cancellationToken)
    {
        TransactionScope transaction = CreateTransactionScope();

        if (!await CheckIfRideExistsAsync(rideId, cancellationToken))
        {
            throw new Exception("Ride doesn't exist");
        }

        await _rideStatusService.ChangeRideStatus(rideId, RideStatus.Confirmed, cancellationToken);

        transaction.Complete();
        transaction.Dispose();
    }

    public async Task CancelRide(long rideId, CancellationToken cancellationToken)
    {
        TransactionScope transaction = CreateTransactionScope();

        if (!await CheckIfRideExistsAsync(rideId, cancellationToken))
        {
            throw new Exception("Ride doesn't exist");
        }

        await _rideStatusService.ChangeRideStatus(rideId, RideStatus.Cancelled, cancellationToken);

        var rideCancelledMessage = new RideCancelledEvent()
        {
            RideId = rideId,
        };
        await _rideProducer.ProduceAsync(rideCancelledMessage, cancellationToken);

        transaction.Complete();
        transaction.Dispose();
    }

    public async Task DriverPositionChanged(DriverStatusDto driverDto, CancellationToken cancellationToken)
    {
        TransactionScope transaction = CreateTransactionScope();

        RideDto? ride = await _rideRepository.GetActiveRideByDriverId(driverDto.DriverId, cancellationToken);

        if (ride == null || !await CheckIfRideExistsAsync(ride.RideId, cancellationToken))
        {
            throw new Exception("Ride doesn't exist");
        }

        switch (ride.Status)
        {
            case RideStatus.Confirmed:
                if (_distanceService.IsNearPickUp(
                        ride.PickupLocation,
                        driverDto.Location,
                        cancellationToken))
                {
                    await _rideStatusService.ChangeRideStatus(ride.RideId, RideStatus.Started, cancellationToken);

                    var rideStartedEvent = new RideStartedEvent()
                    {
                        RideId = ride.RideId,
                    };
                    await _rideProducer.ProduceAsync(rideStartedEvent, cancellationToken);
                }

                break;

            case RideStatus.Started:
                if (_distanceService.IsNearDropoff(
                        ride.PickupLocation,
                        driverDto.Location,
                        cancellationToken))
                {
                    await _rideStatusService.ChangeRideStatus(ride.RideId, RideStatus.Completed, cancellationToken);

                    var rideStartedEvent = new RideCompletedEvent()
                    {
                        RideId = ride.RideId,
                    };
                    await _rideProducer.ProduceAsync(rideStartedEvent, cancellationToken);
                }

                break;
        }

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

    private async Task<bool> CheckIfRideExistsAsync(long rideId, CancellationToken cancellationToken)
    {
        TransactionScope transaction = CreateTransactionScope();

        RideDto? ride = await _rideRepository.GetRide(rideId, cancellationToken);

        transaction.Complete();
        transaction.Dispose();

        return ride != null;
    }
}