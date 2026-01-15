using Application.DTO;
using Application.DTO.Enums;
using Application.Ports;
using Application.Ports.ProducersPorts;
using Application.Ports.ProducersPorts.Events;
using Application.Repositories;
using Application.Services.Interfaces;
using System.Transactions;

namespace Application.Services;

public class RideService : IRideService
{
    private readonly IRideRepository _rideRepository;

    private readonly IRideStatusService _rideStatusService;

    private readonly IRideProducer _rideProducer;

    private readonly IRouteServicePort _routeServicePort;

    public RideService(
        IRideRepository rideRepository,
        IRideStatusService rideStatusService,
        IRideProducer rideProducer,
        IRouteServicePort routeServicePort)
    {
        _rideRepository = rideRepository;
        _rideStatusService = rideStatusService;
        _routeServicePort = routeServicePort;
        _rideProducer = rideProducer;
    }

    public async Task<long> CreateRide(
        long passengerId,
        PointDto pickupLocation,
        PointDto dropOffLocation,
        CancellationToken cancellationToken)
    {
        TransactionScope transaction = CreateTransactionScope();

        if (await GetPersonCurrentRide(passengerId, cancellationToken) != null)
        {
            throw new ApplicationException("Ride already exists");
        }

        var rideDto = new RideDto()
        {
            PassengerId = passengerId,
            PickupLocation = pickupLocation,
            DropLocation = dropOffLocation,
            Status = RideStatus.Requested,
        };

        long rideId = await _rideRepository.CreateRide(rideDto, cancellationToken);

        long routeId = await _routeServicePort.CalculateRoute(pickupLocation, dropOffLocation, cancellationToken);
        await _rideRepository.AddRoute(rideId, routeId, cancellationToken);

        await _rideStatusService.ChangeRideStatus(rideId, RideStatus.SearchingDriver, cancellationToken);

        var rideRequestedEvent = new RideRequestedEvent()
        {
            RideId = rideId,
            PassengerId = passengerId,
            PickupLocation = pickupLocation,
            DropLocation = dropOffLocation,
        };
        await _rideProducer.ProduceAsync(rideRequestedEvent, cancellationToken);

        transaction.Complete();
        transaction.Dispose();

        return rideId;
    }

    public async Task<RideDto?> GetPersonCurrentRide(long passengerId, CancellationToken cancellationToken)
    {
        return await _rideRepository.GetActiveRideByPassengerId(passengerId, cancellationToken);
    }

    public async Task<RideDto?> GetRide(long rideId, CancellationToken cancellationToken)
    {
        return await _rideRepository.GetRide(rideId, cancellationToken);
    }

    private static TransactionScope CreateTransactionScope()
    {
        return new TransactionScope(
            TransactionScopeOption.Required,
            new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted },
            TransactionScopeAsyncFlowOption.Enabled);
    }
}