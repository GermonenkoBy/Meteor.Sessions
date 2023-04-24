using Meteor.Sessions.Api.HealthChecks;
using Meteor.Sessions.Api.Jobs;
using Meteor.Sessions.Core;
using Meteor.Sessions.Infrastructure.Services;
using Meteor.Sessions.Infrastructure.Services.Contracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration.AzureAppConfiguration;
using Microsoft.FeatureManagement;

var builder = WebApplication.CreateBuilder(args);

var azureAppConfigurationConnectionString = builder.Configuration
    .GetValue<string>("ConnectionStrings:AzureAppConfiguration");

if (!string.IsNullOrEmpty(azureAppConfigurationConnectionString))
{
    builder.Configuration.AddAzureAppConfiguration(options =>
        options
            .Connect(azureAppConfigurationConnectionString)
            .UseFeatureFlags()
            .Select(KeyFilter.Any)
            .Select(KeyFilter.Any, builder.Environment.EnvironmentName)
            .Select(KeyFilter.Any, $"{builder.Environment.EnvironmentName}-Sessions")
    );
}

builder.Services.AddFeatureManagement();

builder.Services.AddGrpc();
builder.Services.AddGrpcReflection();

var sessionsConnectionString = builder.Configuration.GetConnectionString("Sessions");
builder.Services.AddDbContext<SessionsContext>(
    options => options
        .UseNpgsql(sessionsConnectionString, opt => opt.MigrationsAssembly("Meteor.Sessions.Migrations"))
        .UseSnakeCaseNamingConvention()
        .EnableSensitiveDataLogging(builder.Environment.IsDevelopment())
);

builder.Services.AddScoped<IMigrationsRunner, MigrationsRunner>();
builder.Services.AddSingleton<MigrationsHealthcheck>();

builder.Services.AddHostedService<MigrationsJob>();

builder.Services.AddHealthChecks()
    .AddCheck<MigrationsHealthcheck>("migrations", tags: new[] { "migrations" });

var app = builder.Build();

app.MapHealthChecks("health", new()
{
    ResponseWriter = ResponseWriters.Json
});
app.MapHealthChecks("health/migrations", new()
{
    Predicate = options => options.Tags.Contains("migrations")
});

app.MapGrpcReflectionService();
app.Run();