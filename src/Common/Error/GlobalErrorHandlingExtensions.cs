using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace Common.Error; 
public static class GlobalErrorHandlingExtensions { // changed to public for external usage

    internal static IServiceCollection AddErrorHandling(this IServiceCollection services) {
        // ProblemDetails hardened config
        services.AddProblemDetails(o => {
            o.CustomizeProblemDetails = ctx => {
                var pd = ctx.ProblemDetails;
                pd.Instance ??= ctx.HttpContext.Request.Path;
                pd.Extensions["traceId"] = ctx.HttpContext.TraceIdentifier;
                if (pd.Status is >= 500)
                    pd.Title ??= "An unexpected error occurred.";
            };
        });

        return services;
    }

    public static WebApplication UseErrorHandling(this WebApplication app) { // renamed from UseErrorHandeling and made public
        // Centralized exception handling: always return RFC7807, never leak internals in prod
        app.UseExceptionHandler(errorApp => {
            errorApp.Run(async context => {
                var env = context.RequestServices.GetRequiredService<IHostEnvironment>();
                var logger = context.RequestServices.GetRequiredService<ILoggerFactory>().CreateLogger("GlobalException");
                var exception = context.Features.Get<IExceptionHandlerFeature>()?.Error;

                var (status, title, code) = exception switch {
                    SecurityTokenException => (StatusCodes.Status401Unauthorized, "Unauthorized", "auth.unauthorized"),
                    UnauthorizedAccessException => (StatusCodes.Status403Forbidden, "Forbidden", "auth.forbidden"),
                    ArgumentException or InvalidOperationException => (StatusCodes.Status400BadRequest, "Bad Request", "request.invalid"),
                    _ => (StatusCodes.Status500InternalServerError, "Server Error", "error.unexpected")
                };

                // Log safely with appropriate level
                if (status >= 500)
                    logger.LogError(exception, "Unhandled exception. TraceId={TraceId}", context.TraceIdentifier);
                else
                    logger.LogWarning(exception, "Handled error {Status}. TraceId={TraceId}", status, context.TraceIdentifier);

                // Prepare RFC7807 response
                var problem = new ProblemDetails {
                    Status = status,
                    Title = title,
                    Type = status >= 500 ? "about:blank" : "https://datatracker.ietf.org/doc/html/rfc9110",
                    Instance = context.Request.Path,
                };

                // Correlation and machine-readable code
                var correlationId = context.Request.Headers["X-Correlation-ID"].ToString();
                if (!string.IsNullOrWhiteSpace(correlationId))
                    problem.Extensions["correlationId"] = correlationId;
                problem.Extensions["traceId"] = context.TraceIdentifier;
                problem.Extensions["code"] = code;

                if (env.IsDevelopment() && exception is not null) {
                    problem.Extensions["exception"] = exception.GetType().Name;
                    problem.Extensions["detail"] = exception.Message;
                }

                // Security/cache headers on error responses
                context.Response.Headers.CacheControl = "no-store";
                context.Response.Headers.Pragma = "no-cache";
                if (status == StatusCodes.Status401Unauthorized)
                    context.Response.Headers.WWWAuthenticate = "Bearer";

                context.Response.StatusCode = status;
                context.Response.ContentType = "application/problem+json";
                await context.Response.WriteAsJsonAsync(problem);
            });
        });
        return app;
    }
}