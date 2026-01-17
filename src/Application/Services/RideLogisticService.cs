using Application.DTO;
using Application.DTO.Enums;
using Application.Ports;
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

    private readonly IRouteServicePort _routeServicePort;

    private readonly IPassengerPort _passengerPort;

    public RideLogisticService(
        IRideRepository rideRepository,
        IRideStatusService rideStatusService,
        IDistanceService distanceService,
        IRideProducer rideProducer,
        IRouteServicePort routeServicePort,
        IPassengerPort passengerPort)
    {
        _rideRepository = rideRepository;
        _rideStatusService = rideStatusService;
        _distanceService = distanceService;
        _rideProducer = rideProducer;
        _routeServicePort = routeServicePort;
        _passengerPort = passengerPort;
    }

    public async Task DriverAssignedAsync(long rideId, long driverId, CancellationToken cancellationToken)
    {
        using TransactionScope transaction = CreateTransactionScope();

        if (!await CheckIfRideExistsAsync(rideId, cancellationToken))
        {
            throw new Exception("Ride doesn't exist");
        }

        await _rideRepository.AddRideDriverAsync(rideId, driverId, cancellationToken);
        await _rideStatusService.ChangeRideStatusAsync(rideId, RideStatus.DriverAssigned, cancellationToken);

        transaction.Complete();
    }

    public async Task DriverAssignationFailedAsync(long rideId, CancellationToken cancellationToken)
    {
        using TransactionScope transaction = CreateTransactionScope();

        if (!await CheckIfRideExistsAsync(rideId, cancellationToken))
        {
            throw new Exception("Ride doesn't exist");
        }

        await _rideStatusService.ChangeRideStatusAsync(rideId, RideStatus.Cancelled, cancellationToken);

        transaction.Complete();
    }

    public async Task RideConfirmedAsync(long rideId, long driverId, CancellationToken cancellationToken)
    {
        using TransactionScope transaction = CreateTransactionScope();

        if (!await CheckIfRideExistsAsync(rideId, cancellationToken))
        {
            throw new Exception("Ride doesn't exist");
        }

        await _rideStatusService.ChangeRideStatusAsync(rideId, RideStatus.Confirmed, cancellationToken);

        var rideAssignedMessage = new RideConfirmedEvent()
        {
            RideId = rideId,
            DriverId = driverId,
        };
        await _rideProducer.ProduceAsync(rideAssignedMessage, cancellationToken);

        transaction.Complete();
    }

    public async Task CancelRideAsync(long rideId, CancellationToken cancellationToken)
    {
        using TransactionScope transaction = CreateTransactionScope();

        if (!await CheckIfRideExistsAsync(rideId, cancellationToken))
        {
            throw new Exception("Ride doesn't exist");
        }

        await _rideStatusService.ChangeRideStatusAsync(rideId, RideStatus.Cancelled, cancellationToken);

        var rideCancelledMessage = new RideCancelledEvent()
        {
            RideId = rideId,
        };
        await _rideProducer.ProduceAsync(rideCancelledMessage, cancellationToken);

        transaction.Complete();
    }

    public async Task DriverPositionChangedAsync(DriverStatusDto driverDto, CancellationToken cancellationToken)
    {
        using TransactionScope transaction = CreateTransactionScope();

        RideDto? ride = await _rideRepository.GetActiveRideByDriverIdAsync(driverDto.DriverId, cancellationToken);

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
                    await _rideStatusService.ChangeRideStatusAsync(ride.RideId, RideStatus.Started, cancellationToken);

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
                    await _rideStatusService.ChangeRideStatusAsync(ride.RideId, RideStatus.Completed, cancellationToken);
                    RouteMetadataDto routeMetadataDto = await _routeServicePort.GetRouteMetadataAsync(ride.RideId, cancellationToken);
                    long passengerAccountId = await _passengerPort.GetPassengerAccountIdAsync(ride.PassengerId, cancellationToken);
                    var rideStartedEvent = new RideCompletedEvent()
                    {
                        AccountId = passengerAccountId,
                        RideId = ride.RideId,
                        DurationMeters = routeMetadataDto.DurationMeters,
                        DurationTime = routeMetadataDto.DurationTime,
                    };
                    await _rideProducer.ProduceAsync(rideStartedEvent, cancellationToken);
                }

                break;
        }

        transaction.Complete();
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
        using TransactionScope transaction = CreateTransactionScope();

        RideDto? ride = await _rideRepository.GetRideAsync(rideId, cancellationToken);

        transaction.Complete();

        return ride != null;
    }
}