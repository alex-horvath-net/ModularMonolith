using Common.Authentication;
using Common.Configuration;
using Common.Error;
using Common.Events;
using Common.HealhCheck;
using Common.Observability;
using Common.Publish;
using Common.RateLimiting;
using Common.Security;
using Common.Version;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Common;

public static class CommonExtensions {
    public static IServiceCollection AddCommon(this IServiceCollection services, IConfiguration config, IHostEnvironment env) {

        services.AddHttps();                    // Enforce Https via HSTS in non-development environments          
        services.AddClientHeadersInProxy();     // Preserve client headers behind proxy, so ratelimiting and redirects can use them
        services.AddRateLimiting();             // Apply rate limiting 
        services.AddApiDocumentation();         // Apply OpenApi 
        services.AddApiAdmin();                 // Apply Swagger Admin
        services.AddApiVersion();               // API Versioning
        services.AddErrorHandling();            // Apply global error handling with RFC7807 ProblemDetails
        services.AddBrowserRequestRestrictions(config);    // restrict incoming HttpRequests from browser
        services.AddJson();           // Harden JSON input to mitigate resource exhaustion & ambiguity
        services.AddAuthentication(config, env);// JWT bearer authentication + strict validation
        services.AddRequestLogging();           // HTTP logging: structured request/response metadata for audit & traceability 
        services.AddObservability(config, env);
        services.AddFullHealthCheck();          // Health checks: basic self check; dependency checks can be added externally for DB/cache etc.
        services.AddAllowedIPsForHealthProbes(config); // IP allowlist filter for health probes

        // Business event publisher (in-process) enables decoupled module interaction without direct references
        services.AddScoped<IBusinessEventPublisher, InProcessBusinessEventPublisher>();

        // Validate critical secrets/config and fail fast in non-development
        services.ValidateSecretsOnStart(config, env);

        return services;
    }


    public static IEndpointRouteBuilder MapCommon(this WebApplication app) {

        app.UseHttps();                 // Enforce Https via HSTS in non-development environments
        app.UseClientHeadersInProxy();  // Preserve client headers behind proxy, so ratelimiting and redirects can use them

        // Move correlation and security headers early so they are present on error responses
        app.UseCorrelationId();         // Inject/propagate correlation ID (X-Correlation-ID)
        app.UseSecurityHeaders();       // CSP + security headers (strict in prod)
        app.UseHttpLogging();           // Request/response metadata logging for audit
        app.UseRateLimiting();          // Apply rate limiting 
        app.UseErrorHandling();         // Apply global error handling with RFC7807 ProblemDetails
        app.UseHttpsRedirection();      // Redirect all HTTP -> HTTPS early
        app.UseBrowserRequestRestrictions();     // restrict incoming HttpRequests From browser
        app.UseAuthentication();        // Validate JWT bearer tokens
        app.UseAuthorization();         // Enforce policies/fallback

        app.MapDevToken();
        app.MapFullHealthCheck();
        app.MapApiDocumentation();      // Apply OpenApi 
        app.MapApiAdmin();              // Apply Swagger Admin

        return app; // Return route builder for further chaining in wbHost Program
    }
}
