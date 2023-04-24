using System.Diagnostics;
using Meteor.Sessions.Core;
using Meteor.Sessions.Infrastructure.Services.Contracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Meteor.Sessions.Infrastructure.Services;

public class MigrationsRunner : IMigrationsRunner
{
    private readonly SessionsContext _context;

    private readonly ILogger<MigrationsRunner> _logger;

    public MigrationsRunner(SessionsContext context, ILogger<MigrationsRunner> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task ApplyAsync(CancellationToken cancellationToken = new())
    {
        var sw = new Stopwatch();
        sw.Start();
        try
        {
            _logger.LogInformation("Applying migrations");
            await _context.Database.MigrateAsync(cancellationToken);
            sw.Stop();
            _logger.LogInformation(
                "Successfully applied migrations, time elapsed: {TimeElapsed}",
                sw.Elapsed.ToString(@"hh\:mm\:ss")
            );
        }
        catch (Exception e)
        {
            _logger.LogCritical(e, "Failed to apply migrations");
            sw.Stop();
            throw;
        }
    }
}