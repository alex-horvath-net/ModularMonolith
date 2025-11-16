using System.Diagnostics;
using Microsoft.AspNetCore.Builder;

namespace Common;

public static class CorrelationIdMiddleware {
    private const string HeaderName = "X-Correlation-ID";

    public static IApplicationBuilder UseCorrelationId(this IApplicationBuilder app) => app.Use(async (context, next) => {
        var correlationID = context.Request.Headers.TryGetValue(HeaderName, out var values) && !string.IsNullOrWhiteSpace(values)
            ? values.ToString()
            : Activity.Current?.Id ?? Guid.NewGuid().ToString("N");

        context.TraceIdentifier = correlationID;
        context.Response.OnStarting(() => {
            context.Response.Headers[HeaderName] = correlationID;
            return Task.CompletedTask;
        });

        await next();
    });
}
