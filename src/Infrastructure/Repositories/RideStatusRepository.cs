using Application.DTO.Enums;
using Application.Repositories;
using Npgsql;

namespace Infrastructure.Repositories;

public class RideStatusRepository : IRideStatusRepository
{
    private readonly NpgsqlDataSource _datasource;

    public RideStatusRepository(NpgsqlDataSource datasource)
    {
        _datasource = datasource;
    }

    public async Task<RideStatus> GetRideStatusAsync(long rideId, CancellationToken token)
    {
        const string sql = """
                        SELECT status
                        FROM rides
                        WHERE ride_id = @ride_id;
                        """;

        await using NpgsqlConnection connection = await _datasource.OpenConnectionAsync(token);
        await using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("ride_id", rideId);

        var result = (RideStatus?)await command.ExecuteScalarAsync(token);

        return result ?? throw new InvalidOperationException($"Ride is not found");
    }

    public async Task ChangeRideStatusAsync(long rideId, RideStatus toRideStatus, CancellationToken token)
    {
        const string sql = """
                        UPDATE rides
                        SET status = @status
                        WHERE ride_id = @ride_id;
                        """;

        await using NpgsqlConnection connection = await _datasource.OpenConnectionAsync(token);
        await using var command = new NpgsqlCommand(sql, connection);

        command.Parameters.AddWithValue("ride_id", rideId);
        command.Parameters.AddWithValue("status", toRideStatus);

        int affected = await command.ExecuteNonQueryAsync(token);
        if (affected == 0)
        {
            throw new InvalidOperationException($"Ride is not found");
        }
    }
}