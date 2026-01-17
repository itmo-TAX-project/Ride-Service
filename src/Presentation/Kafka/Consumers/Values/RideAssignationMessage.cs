namespace Presentation.Kafka.Consumers.Values;

public class RideAssignationMessage
{
    public AssignationStatus AssignationStatus { get; set; }

    public long DriverId { get; set; }
}