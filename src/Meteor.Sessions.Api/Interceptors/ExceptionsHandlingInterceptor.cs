using System.Text.Json;
using Grpc.Core;
using Grpc.Core.Interceptors;
using Meteor.Common.Core.Exceptions;

namespace Meteor.Sessions.Api.Interceptors;

public class ExceptionsHandlingInterceptor : Interceptor
{
    private readonly ILogger<ExceptionsHandlingInterceptor> _logger;

    public ExceptionsHandlingInterceptor(ILogger<ExceptionsHandlingInterceptor> logger)
    {
        _logger = logger;
    }

    public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(
        TRequest request,
        ServerCallContext context,
        UnaryServerMethod<TRequest, TResponse> continuation
    )
    {
        try
        {
            return await continuation(request, context);
        }
        catch (MeteorNotFoundException e)
        {
            _logger.LogInformation("Not found error returned for the following request: {Request}", JsonSerializer.Serialize(request));
            throw new RpcException(new Status(StatusCode.NotFound, e.Message));
        }
        catch (MeteorException e)
        {
            _logger.LogInformation("Error returned for the following request: {Request}", JsonSerializer.Serialize(request));
            throw new RpcException(new Status(StatusCode.InvalidArgument, e.Message, e));
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to handle request: {Request}", JsonSerializer.Serialize(request));
            throw new RpcException(new Status(StatusCode.Unknown, e.Message, e));
        }
    }
}