using FluentMigrator;

namespace Infrastructure.Database.Migrations;

[Migration(1, description: "Creates a ride status enum migration")]
public class CreateRideStatusMigration : Migration
{
    public override void Up()
    {
        Execute.Sql("""
                    CREATE TYPE ride_status AS ENUM (
                        'requested',
                        'searching_driver',
                        'driver_assigned',
                        'confirmed',
                        'started',
                        'completed',
                        'cancelled'
                    );
                    """);
    }

    public override void Down()
    {
        Execute.Sql("""
                    DROP TYPE IF EXISTS ride_status;
                    """);
    }
}