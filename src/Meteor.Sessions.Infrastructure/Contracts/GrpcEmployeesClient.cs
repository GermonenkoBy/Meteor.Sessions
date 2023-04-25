using Grpc.Core;
using MapsterMapper;
using Meteor.Sessions.Core.Contracts;
using Meteor.Sessions.Infrastructure.Grpc;

namespace Meteor.Sessions.Infrastructure.Contracts;

public class GrpcEmployeesClient : IEmployeesClient
{
    private readonly Grpc.EmployeesService.EmployeesServiceClient _grpcClient;

    private readonly IMapper _mapper;

    public GrpcEmployeesClient(EmployeesService.EmployeesServiceClient grpcClient, IMapper mapper)
    {
        _grpcClient = grpcClient;
        _mapper = mapper;
    }

    public async Task<Core.Models.Employee?> GetEmployeeAsync(int customerId, string emailAddress)
    {
        try
        {
            var request = new GetEmployeeByEmailRequest
            {
                EmailAddress = emailAddress,
            };
            var metadata = GetCustomerIdMetadata(customerId);
            var employee = await _grpcClient.GetEmployeeByEmailAsync(request, metadata);
            return _mapper.Map<Core.Models.Employee>(employee);
        }
        catch (RpcException e) when (e.StatusCode == StatusCode.NotFound)
        {
            return null;
        }
    }

    public async Task<bool> VerifyPasswordAsync(int customerId, int employeeId, string password)
    {
        var request = new ValidatePasswordRequest
        {
            EmployeeId = employeeId,
            Password = password,
        };
        var metadata = GetCustomerIdMetadata(customerId);
        var response = await _grpcClient.ValidatePasswordAsync(request, metadata);
        return response.Valid;
    }

    private static Metadata GetCustomerIdMetadata(int customerId)
    {
        return new Metadata
        {
            { "Meteor-Customer-Id", customerId.ToString() }
        };
    }
}