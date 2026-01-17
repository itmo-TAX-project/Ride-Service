using Application.DTO;
using Application.DTO.Enums;
using Application.Repositories;
using Npgsql;

namespace Infrastructure.Repositories;

public class RideRepository : IRideRepository
{
    private readonly NpgsqlDataSource _datasource;

    public RideRepository(NpgsqlDataSource datasource)
    {
        _datasource = datasource;
    }

    public async Task<long> CreateRideAsync(RideDto dto, CancellationToken cancellationToken)
    {
        const string sql = """
                           INSERT INTO rides (
                               passenger_id,
                               pickup_latitude,
                               pickup_longitude,
                               dropoff_latitude,
                               dropoff_longitude,
                               status
                           )
                           VALUES (
                               @passenger_id,
                               @pickup_latitude,
                               @pickup_longitude,
                               @dropoff_latitude,
                               @dropoff_longitude,
                               @status
                           )
                           RETURNING ride_id;
                           """;

        await using NpgsqlConnection connection = await _datasource.OpenConnectionAsync(cancellationToken);
        await using var command = new NpgsqlCommand(sql, connection);

        command.Parameters.AddWithValue("passenger_id", dto.PassengerId);
        command.Parameters.AddWithValue("pickup_latitude", dto.PickupLocation.Latitude);
        command.Parameters.AddWithValue("pickup_longitude", dto.PickupLocation.Longitude);
        command.Parameters.AddWithValue("dropoff_latitude", dto.DropLocation.Latitude);
        command.Parameters.AddWithValue("dropoff_longitude", dto.DropLocation.Longitude);
        command.Parameters.AddWithValue("status", dto.Status);

        return (long)(await command.ExecuteScalarAsync(cancellationToken)
                      ?? throw new InvalidOperationException("Can not create ride"));
    }

    public async Task<RideDto?> GetRideAsync(long rideId, CancellationToken cancellationToken)
    {
        const string sql = """
                           SELECT *
                           FROM rides
                           WHERE ride_id = @ride_id;
                           """;

        await using NpgsqlConnection connection = await _datasource.OpenConnectionAsync(cancellationToken);
        await using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("ride_id", rideId);

        await using NpgsqlDataReader reader = await command.ExecuteReaderAsync(cancellationToken);
        if (!await reader.ReadAsync(cancellationToken))
        {
            return null;
        }

        return new RideDto
        {
            RideId = reader.GetInt64(0),
            PassengerId = reader.GetInt64(1),
            PickupLocation = new PointDto(reader.GetDouble(2), reader.GetDouble(3)),
            DropLocation = new PointDto(reader.GetDouble(4), reader.GetDouble(5)),
            Status = await reader.GetFieldValueAsync<RideStatus>(6, cancellationToken),
            AssignedDriverId = await reader.IsDBNullAsync(7, cancellationToken) ? null : reader.GetInt64(7),
            RouteId = await reader.IsDBNullAsync(8, cancellationToken) ? null : reader.GetInt64(8),
        };
    }

    public async Task AddRideDriverAsync(long rideId, long driverId, CancellationToken cancellationToken)
    {
        const string sql = """
                           UPDATE rides
                           SET assigned_driver_id = @driver_id
                           WHERE ride_id = @ride_id;
                           """;

        await using NpgsqlConnection connection = await _datasource.OpenConnectionAsync(cancellationToken);
        await using var command = new NpgsqlCommand(sql, connection);

        command.Parameters.AddWithValue("ride_id", rideId);
        command.Parameters.AddWithValue("driver_id", driverId);

        await command.ExecuteNonQueryAsync(cancellationToken);
    }

    public async Task AddRouteAsync(long rideId, long routeId, CancellationToken cancellationToken)
    {
        const string sql = """
                           UPDATE rides
                           SET route_id = @route_id
                           WHERE ride_id = @ride_id;
                           """;

        await using NpgsqlConnection connection = await _datasource.OpenConnectionAsync(cancellationToken);
        await using var command = new NpgsqlCommand(sql, connection);

        command.Parameters.AddWithValue("ride_id", rideId);
        command.Parameters.AddWithValue("route_id", routeId);

        await command.ExecuteNonQueryAsync(cancellationToken);
    }

    public async Task<RideDto?> GetActiveRideByPassengerIdAsync(long passengerId, CancellationToken cancellationToken)
    {
        const string sql = """
                           SELECT *
                           FROM rides
                           WHERE passenger_id = @passenger_id
                             AND status NOT IN ('completed', 'cancelled')
                           LIMIT 1;
                           """;

        await using NpgsqlConnection connection = await _datasource.OpenConnectionAsync(cancellationToken);
        await using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("passenger_id", passengerId);

        await using NpgsqlDataReader reader = await command.ExecuteReaderAsync(cancellationToken);
        if (!await reader.ReadAsync(cancellationToken))
        {
            return null;
        }

        return new RideDto
        {
            RideId = reader.GetInt64(0),
            PassengerId = reader.GetInt64(1),
            PickupLocation = new PointDto(reader.GetDouble(2), reader.GetDouble(3)),
            DropLocation = new PointDto(reader.GetDouble(4), reader.GetDouble(5)),
            Status = await reader.GetFieldValueAsync<RideStatus>(6, cancellationToken),
            AssignedDriverId = await reader.IsDBNullAsync(7, cancellationToken) ? null : reader.GetInt64(7),
            RouteId = await reader.IsDBNullAsync(8, cancellationToken) ? null : reader.GetInt64(8),
        };
    }

    public async Task<RideDto?> GetActiveRideByDriverIdAsync(long driverId, CancellationToken cancellationToken)
    {
        const string sql = """
                           SELECT *
                           FROM rides
                           WHERE assigned_driver_id = @driver_id
                             AND status NOT IN ('completed', 'cancelled')
                           LIMIT 1;
                           """;

        await using NpgsqlConnection connection = await _datasource.OpenConnectionAsync(cancellationToken);
        await using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("driver_id", driverId);

        await using NpgsqlDataReader reader = await command.ExecuteReaderAsync(cancellationToken);
        if (!await reader.ReadAsync(cancellationToken))
        {
            return null;
        }

        return new RideDto
        {
            RideId = reader.GetInt64(0),
            PassengerId = reader.GetInt64(1),
            PickupLocation = new PointDto(reader.GetDouble(2), reader.GetDouble(3)),
            DropLocation = new PointDto(reader.GetDouble(4), reader.GetDouble(5)),
            Status = await reader.GetFieldValueAsync<RideStatus>(6, cancellationToken),
            AssignedDriverId = await reader.IsDBNullAsync(7, cancellationToken) ? null : reader.GetInt64(7),
            RouteId = await reader.IsDBNullAsync(8, cancellationToken) ? null : reader.GetInt64(8),
        };
    }
}