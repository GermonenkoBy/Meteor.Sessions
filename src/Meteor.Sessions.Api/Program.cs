using Grpc.Core;
using Mapster;
using MapsterMapper;
using Meteor.Sessions.Api.HealthChecks;
using Meteor.Sessions.Api.Jobs;
using Meteor.Sessions.Core;
using Meteor.Sessions.Core.Contracts;
using Meteor.Sessions.Core.Services;
using Meteor.Sessions.Core.Services.Contracts;
using Meteor.Sessions.Infrastructure.Contracts;
using Meteor.Sessions.Infrastructure.Grpc;
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
builder.Services.AddGrpcClient<CustomersService.CustomersServiceClient>(options =>
{
    var url = builder.Configuration.GetValue<string>("Routing:ControllerUrl") ?? string.Empty;
    options.Address = new Uri(url);
    if (options.Address.Scheme.Equals("http", StringComparison.OrdinalIgnoreCase))
    {
        options.ChannelOptionsActions.Add(opt => opt.Credentials = ChannelCredentials.Insecure);
    }
});
builder.Services.AddGrpcClient<EmployeesService.EmployeesServiceClient>(options =>
{
    var url = builder.Configuration.GetValue<string>("Routing:EmployeesServiceUrl") ?? string.Empty;
    options.Address = new Uri(url);
    if (options.Address.Scheme.Equals("http", StringComparison.OrdinalIgnoreCase))
    {
        options.ChannelOptionsActions.Add(opt => opt.Credentials = ChannelCredentials.Insecure);
    }
});

var sessionsConnectionString = builder.Configuration.GetConnectionString("Sessions");
builder.Services.AddDbContext<SessionsContext>(
    options => options
        .UseNpgsql(sessionsConnectionString, opt => opt.MigrationsAssembly("Meteor.Sessions.Migrations"))
        .UseSnakeCaseNamingConvention()
        .EnableSensitiveDataLogging(builder.Environment.IsDevelopment())
);

builder.Services.AddScoped<ISessionsService, SessionsService>();
builder.Services.AddScoped<ICustomersClient, GrpcCustomersClient>();
builder.Services.AddScoped<IEmployeesClient, GrpcEmployeesClient>();
builder.Services.AddScoped<IRefreshTokenGenerator, RefreshTokenGenerator>();
builder.Services.AddScoped<IMigrationsRunner, MigrationsRunner>();

var mapperConfig = new TypeAdapterConfig();
mapperConfig.Apply(new Meteor.Sessions.Infrastructure.Mapping.MappingRegister());
builder.Services.AddSingleton<IMapper>(new Mapper(mapperConfig));

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