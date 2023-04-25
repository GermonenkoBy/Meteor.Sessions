namespace Meteor.Sessions.Infrastructure.Services.Contracts;

public interface IMigrationsRunner
{
    Task ApplyAsync(CancellationToken cancellationToken = new());
}