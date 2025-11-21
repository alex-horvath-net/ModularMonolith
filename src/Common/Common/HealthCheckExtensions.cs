using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Common;

internal static class HealthCheckExtensions {

    public static IServiceCollection AddAllowedIPsForHealthProbes(this IServiceCollection services, IConfiguration configuration) {
        // Rename from AddAllowdIPsForHealthProbes; register concrete filter so generic AddEndpointFilter<T>() can resolve it directly.
        var allowedIps = configuration.GetSection("Health:AllowedIps").Get<string[]>() ?? Array.Empty<string>();
        services.AddSingleton<IpEndpointFilter>(new IpEndpointFilter(allowedIps));
        return services;
    }

    internal static IServiceCollection AddFullHealthCheck(this IServiceCollection services) {
        // Rename from AddFullHealthChek
        services.AddHealthChecks()
            .AddCheck("self", () => HealthCheckResult.Healthy());

        return services;
    }
    internal static IEndpointRouteBuilder MapFullHealthCheck(this WebApplication app) {

        // Re-implement health endpoints as minimal route handlers so endpoint filters can be used.

        var group = app.MapGroup("/health")
            .WithTags("Health")
            .AllowAnonymous()
            .AddEndpointFilter<IpEndpointFilter>()
            .DisableRateLimiting();

        group.MapGet("/live", GetLive); // Live: quick self check only

        group.MapGet("/ready", GetReady); // Ready: all registered checks

        return app;
    }

    private static async Task<IResult> GetLive(HealthCheckService health)  {
        var report = await health.CheckHealthAsync(resgistration => resgistration.Name == "self");
        var payload = new {
            status = report.Status.ToString(),
            checks = report.Entries.Select(entry => new { 
                name = entry.Key, 
                status = entry.Value.Status.ToString(), 
                duration = entry.Value.Duration 
            })
        };
        return report.Status == HealthStatus.Healthy
            ? Results.Json(payload)
            : Results.StatusCode(StatusCodes.Status503ServiceUnavailable);
    }

    private static async Task<IResult> GetReady(HealthCheckService health) {
        var report = await health.CheckHealthAsync(resgistration => true);
        var payload = new {
            status = report.Status.ToString(),
            checks = report.Entries.Select(entry => new { 
                name = entry.Key, 
                status = entry.Value.Status.ToString(), 
                duration = entry.Value.Duration })
        };
        return report.Status == HealthStatus.Healthy
            ? Results.Json(payload)
            : Results.StatusCode(StatusCodes.Status503ServiceUnavailable);
    }

} 