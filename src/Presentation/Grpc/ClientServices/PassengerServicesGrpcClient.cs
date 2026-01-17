using Application.Ports;
using PassengerMaster.Grpc;

namespace Presentation.Grpc.ClientServices;

public class PassengerServicesGrpcClient : IPassengerPort
{
    private readonly PassengerService.PassengerServiceClient _serviceClient;

    public PassengerServicesGrpcClient(PassengerService.PassengerServiceClient serviceClient)
    {
        _serviceClient = serviceClient;
    }

    public async Task<long> GetPassengerAccountIdAsync(long passengerId, CancellationToken cancellationToken)
    {
        GetAccountIdResponse result = await _serviceClient.GetAccountIdAsync(new GetAccountIdRequest() { PassengerId = passengerId }, cancellationToken: cancellationToken);
        return result.AccountId;
    }
}