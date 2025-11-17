using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;

namespace Common;

public static class CommonExtensions {
    // Plan (pseudocode):
    // - Extract forwarded headers configuration (Configure<ForwardedHeadersOptions>) into a dedicated extension:
    //     AddDefaultForwardedHeaders(IServiceCollection)
    //   Why:
    //     - Centralizes proxy trust settings to avoid copy/paste and drift across services.
    //     - Keeps Program/Composition root concise and auditable.
    //     - Ensures consistent security posture around X-Forwarded-* handling.
    //   What:
    //     - Enable processing of X-Forwarded-For and X-Forwarded-Proto headers.
    //     - Clear KnownNetworks and KnownProxies to avoid implicit trusts (infra should control trusted sources).
    //     - Leave UseForwardedHeaders() order unchanged in the request pipeline.
    //   Risks / Guidance:
    //     - Only enable forwarded headers when running behind a trusted reverse proxy (ingress/controller/LB).
    //     - Infra (e.g., Nginx/Envoy/ALB) should sanitize/append X-Forwarded-* correctly to prevent spoofing.
    //     - If you need to restrict to specific networks/proxies, configure KnownNetworks/KnownProxies explicitly.
    // - Replace inline block in AddCommon with services.AddDefaultForwardedHeaders().
    // - Keep all other code intact (rate limiting, auth, CORS, health checks, etc.).

    // Overload with environment to keep host Program.cs lean; delegates to primary AddCommon
    public static IServiceCollection AddCommon(this IServiceCollection services, IConfiguration configuration, IHostEnvironment environment) {

        services.AddOpenApi();              // Registers OpenAPI/Swagger generation (dev only exposure later)
        services.AddProblemDetails();       // Enables RFC7807 standardized error responses

        services.AddStrictHttpJson();     // Harden JSON input to mitigate resource exhaustion & ambiguity

        services.AddAllowdOriginsForBrowser(configuration);

        services.AddAuthenticationAndAuthorization(configuration); // JWT bearer authentication + strict validation

        services.AddDefaultHttpLogging();   // HTTP logging: structured request/response metadata for audit & traceability 

        services.AddDefaultRateLimiting();  // Global + targeted rate limiting policies

        // Health checks: basic self check; dependency checks can be added externally for DB/cache etc.
        services.AddHealthChecks().AddCheck("self", () => HealthCheckResult.Healthy());

        // Forwarded headers: trust reverse proxy for original scheme/client IP (ingress must sanitize/XFF chain)
        services.AddDefaultForwardedHeaders();

        // Business event publisher (in-process) enables decoupled module interaction without direct references
        services.AddScoped<IBusinessEventPublisher, InProcessBusinessEventPublisher>();

        services.AddAllowdIPsForHealthProbes(configuration);

        return services;
    }

    public static IEndpointRouteBuilder MapCommon(this WebApplication app) {
        // Enforce HSTS (strict transport) in non-development environments
        if (!app.Environment.IsDevelopment())
            app.UseHsts();

        app.UseRateLimiter();          // Apply global rate limiting middleware

        if (app.Environment.IsDevelopment())
            app.MapOpenApi(); // Expose OpenAPI only in dev

        app.UseExceptionHandler(_ => { }); // Centralized exception handling -> ProblemDetails formatting
        app.UseHttpsRedirection();         // Redirect all HTTP -> HTTPS early
        app.UseForwardedHeaders();         // Process X-Forwarded-* headers from trusted proxy
        app.UseCorrelationId();            // Inject/propagate correlation ID (X-Correlation-ID)
        app.UseSecurityHeaders(app.Environment.IsDevelopment()); // CSP + security headers (strict in prod)
        app.UseCors("DefaultCors");      // Apply configured CORS policy
        app.UseHttpLogging();             // Request/response metadata logging for audit
        app.UseAuthentication();          // Validate JWT bearer tokens
        app.UseAuthorization();           // Enforce policies/fallback

        if (app.Environment.IsDevelopment()) {
            var group = app.MapGroup("/auth").WithTags("Auth").AllowAnonymous(); // Dev-only auth helper
            group.MapPost("/dev-token", CreateDevToken);                         // Issue short-lived dev token
        }

        // Health endpoints: anonymous by design for orchestrator probes; attach IP filter if configured
        var ipFilter = app.Services.GetServices<IEndpointFilter>().OfType<AllowedIpHealthFilter>().FirstOrDefault();
        var live = app.MapHealthChecks("/health/live").AllowAnonymous();
        var ready = app.MapHealthChecks("/health/ready").AllowAnonymous();
        if (ipFilter is not null) {
            live.AddEndpointFilter(ipFilter);
            ready.AddEndpointFilter(ipFilter);
        }

        return app; // Return route builder for further chaining in host Program
    }

    private static IResult CreateDevToken(IConfiguration configuration) {
        // Extract token issuance parameters from configuration
        var audience = configuration["Auth:Audience"]!;
        var issuer = configuration["Auth:Issuer"]!;
        var devKey = configuration["Auth:DevKey"]!;
        var devScopes = configuration.GetSection("Auth:DevScopes").Get<string[]>() ?? Array.Empty<string>(); // Scoped permissions for dev token

        var handler = new JwtSecurityTokenHandler();

        // Standard claims set for minimal bearer token
        var claims = new List<Claim> {
            new(JwtRegisteredClaimNames.Sub, "dev-user"),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString("N")),
            new(JwtRegisteredClaimNames.Iat, EpochTime.GetIntDate(DateTime.UtcNow).ToString(), ClaimValueTypes.Integer64)
        };
        claims.AddRange(devScopes.Select(devScope => new Claim("scope", devScope)));
        var subject = new ClaimsIdentity(claims);
        var signingCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(devKey)), SecurityAlgorithms.HmacSha256);

        // Create short-lived access token
        var token = handler.CreateJwtSecurityToken(
            issuer: issuer,
            audience: audience,
            subject: subject,
            notBefore: DateTime.UtcNow,
            expires: DateTime.UtcNow.AddMinutes(10),
            signingCredentials: signingCredentials
        );

        return Results.Ok(new { access_token = handler.WriteToken(token), token_type = "Bearer", expires_in = 600 });
    }

    public static IServiceCollection AddAllowdIPsForHealthProbes(this IServiceCollection services, IConfiguration configuration) {
        // IP allowlist filter for health probes (optional security enhancement) - configured via Health:AllowedIps
        var allowedIps = configuration.GetSection("Health:AllowedIps").Get<string[]>() ?? Array.Empty<string>();
        services.AddSingleton<IEndpointFilter>(new AllowedIpHealthFilter(allowedIps));
        return services;
    }

    public static IServiceCollection AddAuthenticationAndAuthorization(this IServiceCollection services, IConfiguration configuration) {
        var audience = configuration["Auth:Audience"]!;
        var issuer = configuration["Auth:Issuer"]!;
        var devKey = configuration["Auth:DevKey"]!;

        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options => {
                options.MapInboundClaims = false;
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters = new TokenValidationParameters {
                    ValidateIssuer = true,
                    ValidIssuer = issuer,
                    ValidateAudience = true,
                    ValidAudience = audience,
                    ValidateLifetime = true,
                    RequireExpirationTime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(devKey)),
                    ClockSkew = TimeSpan.FromSeconds(30),
                    ValidTypes = new[] { "JWT", "at+jwt" },
                    ValidAlgorithms = new[] { SecurityAlgorithms.HmacSha256 }
                };
            });

        services
            .AddAuthorizationBuilder()
            .SetFallbackPolicy(new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build());

        return services;
    }

    private static IServiceCollection AddStrictHttpJson(this IServiceCollection services) {
        services.ConfigureHttpJsonOptions(o => {
            o.SerializerOptions.ReadCommentHandling = JsonCommentHandling.Disallow;
            o.SerializerOptions.MaxDepth = 32;
            o.SerializerOptions.PropertyNameCaseInsensitive = false;
        });
        return services;
    }

    public static IServiceCollection AddAllowdOriginsForBrowser(this IServiceCollection services, IConfiguration configuration) {
        var origins = configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? Array.Empty<string>();

        services.AddCors(options => {
            options.AddPolicy("DefaultCors", policy => {
                if (origins.Length > 0) {
                    policy.WithOrigins(origins)
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                }
            });
        });
        return services;
    }

    public static IServiceCollection AddDefaultHttpLogging(this IServiceCollection services) {
        services.AddHttpLogging(options => {
            options.LoggingFields =
                HttpLoggingFields.Request |
                HttpLoggingFields.Response |
                HttpLoggingFields.Duration;
        });
        return services;
    }

    // Rate limiting policy extracted for reuse and cleanliness
    public static IServiceCollection AddDefaultRateLimiting(this IServiceCollection services) {
        services.AddRateLimiter(options => {
            // Time-based throughput control
            options.AddFixedWindowLimiter("fixed", o => {
                o.PermitLimit = 100;                       // Allow 100 executions per window
                o.Window = TimeSpan.FromSeconds(10);       // Each window = 10 seconds
                o.QueueProcessingOrder = QueueProcessingOrder.OldestFirst; // Fair: first waiting goes first
                o.QueueLimit = 50;                         // Up to 50 over-limit requests can wait
            });

            // Simultaneous execution control (resource pressure safety)
            options.AddConcurrencyLimiter("writes", o => {
                o.PermitLimit = 10;    // At most 10 concurrent executions
                o.QueueLimit = 20;     // Up to 20 more may wait for a slot
            });

            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
            options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
            {
                // Apply fixed window limiter globally (by client IP)
                var clientIp = httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
                return RateLimitPartition.GetFixedWindowLimiter(clientIp, _ => new FixedWindowRateLimiterOptions
                {
                    PermitLimit = 100,
                    Window = TimeSpan.FromSeconds(10),
                    QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                    QueueLimit = 50
                });
            });
        });
        return services;
    }

    /// <summary>
    /// Applies secure defaults for processing reverse-proxy forwarded headers.
    /// Deep explanation:
    /// - Purpose: Preserve original client IP and scheme when behind a proxy (Kestrel otherwise sees the proxy).
    /// - Headers: X-Forwarded-For (client/public IP chain) and X-Forwarded-Proto (original scheme: http/https).
    /// - Trust model: We clear KnownNetworks/KnownProxies to avoid accidental implicit trusts. Your ingress/
    ///   gateway must strip untrusted X-Forwarded-* from the edge and append its own, ensuring integrity.
    /// - Customization:
    ///     * If your environment requires restricting the trusted sources, explicitly set KnownProxies or
    ///       KnownNetworks here with IP ranges of your LB/ingress. Do not trust arbitrary sources.
    /// - Pipeline: Keep app.UseForwardedHeaders() early in the middleware pipeline (before auth, redirects,
    ///   link generation) so downstream components receive correct Scheme/RemoteIpAddress.
    /// </summary>
    public static IServiceCollection AddDefaultForwardedHeaders(this IServiceCollection services) {
        services.Configure<ForwardedHeadersOptions>(options => {
            options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
            options.KnownNetworks.Clear(); // Explicit trust configuration handled at infrastructure level
            options.KnownProxies.Clear();  // Avoid implicit trusts
            // NOTE: If required, you can add:
            // options.KnownProxies.Add(IPAddress.Parse("10.0.0.10"));
            // or:
            // options.KnownNetworks.Add(new IPNetwork(IPAddress.Parse("10.0.0.0"), 8));
        });
        return services;
    }
}