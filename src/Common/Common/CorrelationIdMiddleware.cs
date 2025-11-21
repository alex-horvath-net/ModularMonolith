using System.Diagnostics;
using Microsoft.AspNetCore.Builder;

namespace Common;

public static class CorrelationIdMiddleware {

    public static IApplicationBuilder UseCorrelationId(this IApplicationBuilder app) => app.Use(async (context, next) => {
        var key = "X-Correlation-ID";
        var hasCorrelationIDHeader =
            context.Request.Headers.TryGetValue(key, out var values) &&
            values.Count == 1 &&
            !string.IsNullOrWhiteSpace(values.SingleOrDefault()?.ToString());

        var correlationID = hasCorrelationIDHeader
            ? values.SingleOrDefault()?.ToString()
            : Activity.Current?.Id ??
              Guid.NewGuid().ToString("N");

        context.TraceIdentifier = correlationID!;
        context.Response.OnStarting(() => {
            context.Response.Headers[key] = correlationID;
            return Task.CompletedTask;
        });

        await next();
    });
}
