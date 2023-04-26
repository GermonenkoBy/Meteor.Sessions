using Meteor.Sessions.Core.Dtos;
using Meteor.Sessions.Core.Models;

namespace Meteor.Sessions.Core.Services.Contracts;

public interface ISessionsService
{
    Task<Session> StartSessionAsync(StartSessionDto sessionDto);

    Task<Session> RefreshTokenAsync(Guid id);

    Task<Session> RefreshTokenAsync(string token);

    Task TerminateSessionAsync(string token);

    Task TerminateSessionAsync(Guid id);
}