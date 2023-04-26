using Grpc.Core;
using MapsterMapper;
using Meteor.Sessions.Api.Grpc;
using Meteor.Sessions.Core.Dtos;
using Meteor.Sessions.Core.Services.Contracts;

namespace Meteor.Sessions.Api.Services;

public class SessionsGrpcService : SessionsService.SessionsServiceBase
{
    private readonly ISessionsService _sessionsService;

    private readonly IMapper _mapper;

    public SessionsGrpcService(ISessionsService sessionsService, IMapper mapper)
    {
        _sessionsService = sessionsService;
        _mapper = mapper;
    }

    public override async Task<Session> StartSession(
        StartSessionRequest request,
        ServerCallContext context
    )
    {
        var sessionDto = _mapper.Map<StartSessionDto>(request);
        var session = await _sessionsService.StartSessionAsync(sessionDto);
        return _mapper.Map<Session>(session);
    }

    public override async Task<Session> RefreshToken(
        SessionIdentifier request,
        ServerCallContext context
    )
    {
        if (request.IdentifierCase == SessionIdentifier.IdentifierOneofCase.Id)
        {
            if (Guid.TryParse(request.Id, out var id))
            {
                var session = await _sessionsService.RefreshTokenAsync(id);
                return _mapper.Map<Session>(session);
            }

            throw new RpcException(new Status(StatusCode.InvalidArgument, "Session ID must be a valid UUID."));
        }

        if (request.IdentifierCase == SessionIdentifier.IdentifierOneofCase.Id)
        {
            var session = await _sessionsService.RefreshTokenAsync(request.Token);
            return _mapper.Map<Session>(session);
        }

        throw new RpcException(new Status(StatusCode.InvalidArgument, "Session ID or refresh token must be provided."));
    }

    public override async Task<StringResponse> TerminateSession(SessionIdentifier request, ServerCallContext context)
    {
        if (request.IdentifierCase == SessionIdentifier.IdentifierOneofCase.Id)
        {
            if (Guid.TryParse(request.Id, out var id))
            {
                await _sessionsService.TerminateSessionAsync(id);
                return new (){Message = "Session terminated."};
            }

            throw new RpcException(new Status(StatusCode.InvalidArgument, "Session ID must be a valid UUID."));
        }

        if (request.IdentifierCase == SessionIdentifier.IdentifierOneofCase.Id)
        {
            await _sessionsService.TerminateSessionAsync(request.Token);
            return new (){Message = "Session terminated."};
        }

        throw new RpcException(new Status(StatusCode.InvalidArgument, "Session ID or refresh token must be provided."));
    }
}