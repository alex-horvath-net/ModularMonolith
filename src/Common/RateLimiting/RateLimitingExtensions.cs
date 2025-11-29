using System.Security.Claims;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace Common.RateLimiting; 
internal static class RateLimitingExtensions {

    internal static IServiceCollection AddRateLimiting(this IServiceCollection services) {
        services.AddRateLimiter(options => {

            // Concurrency guard for write endpoints
            options.AddConcurrencyLimiter("writes", o => {
                o.PermitLimit = 10;   // Max concurrent writes
                o.QueueLimit = 10;    // Small queue to cap latency, =1 is rejected
            });

            // Global identity/tenant/user oriented throughput control (sliding window for smoothing)
            options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext => {
                var user = httpContext.User;
                var identityKey =
                    user?.FindFirst("sub")?.Value ??
                    user?.FindFirst("client_id")?.Value ??
                    user?.FindFirst(ClaimTypes.NameIdentifier)?.Value ??
                    httpContext.Connection.RemoteIpAddress?.ToString() ??
                    "anonymous";

                return RateLimitPartition.GetSlidingWindowLimiter(identityKey, _ => new SlidingWindowRateLimiterOptions {
                    PermitLimit = 100,
                    Window = TimeSpan.FromSeconds(10),
                    SegmentsPerWindow = 10,
                    QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                    QueueLimit = 20
                });
            });

            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

            options.OnRejected = async (ctx, token) => {
                if (ctx.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfter))
                    ctx.HttpContext.Response.Headers.RetryAfter = ((int)retryAfter.TotalSeconds).ToString();
                await ctx.HttpContext.Response.WriteAsync("Too many requests", token);
            };
        });
        return services;
    }

    internal static IEndpointRouteBuilder UseRateLimiting(this WebApplication app) {
        app.UseRateLimiter();          // Apply global rate limiting middleware

        return app;
    }
}