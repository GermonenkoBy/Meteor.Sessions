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
        throw new NotImplementedException();
    }

    public Task<bool> VerifyPasswordAsync(int customerId, int employeeId, string password)
    {
        throw new NotImplementedException();
    }
}