using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Meteor.Sessions.Api.HealthChecks;

public class ResponseWriters
{
    public static readonly Func<HttpContext, HealthReport, Task> Json = async (context, report) =>
    {
        Dictionary<string, object> healthReport = new();
        foreach (var (healthCheckName, value) in report.Entries)
        {
            healthReport[healthCheckName] = new
            {
                status = value.Status.ToString().ToLower(),
                message = value.Description,
            };
        }

        await context.Response.WriteAsJsonAsync(healthReport);
    };
}