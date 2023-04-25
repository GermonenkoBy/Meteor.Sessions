using Grpc.Core;
using MapsterMapper;
using Meteor.Sessions.Core.Contracts;
using Meteor.Sessions.Infrastructure.Grpc;

namespace Meteor.Sessions.Infrastructure.Contracts;

public class GrpcCustomersClient: ICustomersClient
{
    private readonly CustomersService.CustomersServiceClient _grpcClient;

    private readonly IMapper _mapper;

    public GrpcCustomersClient(CustomersService.CustomersServiceClient grpcClient, IMapper mapper)
    {
        _grpcClient = grpcClient;
        _mapper = mapper;
    }

    public async Task<Core.Models.Customer?> GetCustomerAsync(string domain)
    {
        try
        {
            var customer = await _grpcClient.GetCustomerAsync(new() { Domain = domain });
            return _mapper.Map<Core.Models.Customer>(customer);
        }
        catch (RpcException e) when(e.StatusCode == StatusCode.NotFound)
        {
            return null;
        }
    }

    public async Task<Core.Models.CustomerSettings?> GetCustomerSettingsAsync(int customerId)
    {
        try
        {
            var customer = await _grpcClient.GetCustomerSettingsAsync(new() {CustomerId = customerId});
            return _mapper.Map<Core.Models.CustomerSettings>(customer);
        }
        catch (RpcException e) when(e.StatusCode == StatusCode.NotFound)
        {
            return null;
        }
    }
}