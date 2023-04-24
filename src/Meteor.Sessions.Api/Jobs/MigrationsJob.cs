using Meteor.Sessions.Api.Enums;
using Meteor.Sessions.Api.HealthChecks;
using Meteor.Sessions.Infrastructure.Services.Contracts;
using Microsoft.FeatureManagement;

namespace Meteor.Sessions.Api.Jobs;

public class MigrationsJob : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;

    private readonly IFeatureManager _featureManager;

    private readonly MigrationsHealthcheck _migrationsHealthcheck;

    public MigrationsJob(
        IServiceProvider serviceProvider,
        IFeatureManager featureManager,
        MigrationsHealthcheck migrationsHealthcheck)
    {
        _serviceProvider = serviceProvider;
        _featureManager = featureManager;
        _migrationsHealthcheck = migrationsHealthcheck;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (!await _featureManager.IsEnabledAsync("RunMigrationsOnStartup"))
        {
            _migrationsHealthcheck.MigrationsStatus = MigrationsStatuses.Disabled;
            return;
        }

        using var scope = _serviceProvider.CreateScope();
        var migrationsRunner = scope.ServiceProvider.GetRequiredService<IMigrationsRunner>();

        try
        {
            _migrationsHealthcheck.MigrationsStatus = MigrationsStatuses.InProgress;
            await migrationsRunner.ApplyAsync(stoppingToken);
            _migrationsHealthcheck.MigrationsStatus = MigrationsStatuses.Completed;
        }
        catch (Exception)
        {
            _migrationsHealthcheck.MigrationsStatus = MigrationsStatuses.Error;
        }
    }
}