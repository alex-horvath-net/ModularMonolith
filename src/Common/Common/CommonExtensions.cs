using System.IdentityModel.Tokens.Jwt;
using System.Security.Authentication;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Server.Kestrel.Https;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace Common;

public static class CommonExtensions {
    
    public static ConfigureWebHostBuilder UseSecureKerstel(this ConfigureWebHostBuilder wbHost) {

        wbHost.UseKestrel(options =>
        {
            options.AddServerHeader = false; // Hide Server header
            options.Limits.MaxRequestBodySize = 10 * 1024 * 1024; // 10MB

            // Enforce TLS 1.2/1.3 only when HTTPS is enabled
            options.ConfigureHttpsDefaults(https =>
            { 
                https.SslProtocols = SslProtocols.Tls13 | SslProtocols.Tls12;
                https.CheckCertificateRevocation = true;
                https.ClientCertificateMode = ClientCertificateMode.NoCertificate;
            });
        });

        return wbHost;
    }
    public static IServiceCollection AddCommon(this IServiceCollection services, IConfiguration config, IHostEnvironment env) {
        services.AddHttpsViaHsts();
        services.AddClientHeadersInProxy();   // Forwarded headers: trust reverse proxy for original scheme/client IP (ingress must sanitize/XFF chain)
        services.AddRateLimiting();   // Global + targeted rate limiting policies


        services.AddOpenApi("document", options => {
            // Add JWT Bearer security scheme so Swagger UI shows the Authorize button
            options.AddDocumentTransformer((document, ctx, ct) => {
                document.Components ??= new();
                document.Components.SecuritySchemes["Bearer"] = new OpenApiSecurityScheme {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Input: Bearer {token}"
                };
                return Task.CompletedTask;
            });
            options.AddOperationTransformer((operation, ctx, ct) => {
                // Apply Bearer auth requirement to all operations (Swagger UI will send JWT when authorized)
                operation.Security ??= new List<OpenApiSecurityRequirement>();
                operation.Security.Add(new OpenApiSecurityRequirement {
                    [
                        new OpenApiSecurityScheme {
                            Reference = new OpenApiReference {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        }
                    ] = Array.Empty<string>()
                });
                return Task.CompletedTask;
            });
        });              // Registers OpenAPI/Swagger generation (dev only exposure later)
        services.AddProblemDetails();       // Enables RFC7807 standardized error responses
        services.AddSecureHttpJson();       // Harden JSON input to mitigate resource exhaustion & ambiguity
        services.AddSecureBrowser(config);  //
        services.AddSecureToken(config, env);    // JWT bearer authentication + strict validation
        services.AddSecureHttpLogging();    // HTTP logging: structured request/response metadata for audit & traceability 

        // Health checks: basic self check; dependency checks can be added externally for DB/cache etc.
        services.AddHealthChecks().AddCheck("self", () => HealthCheckResult.Healthy());
        services.AddAllowdIPsForHealthProbes(config); // // IP allowlist filter for health probes

        // Business event publisher (in-process) enables decoupled module interaction without direct references
        services.AddScoped<IBusinessEventPublisher, InProcessBusinessEventPublisher>();

        return services;
    }

    public static IEndpointRouteBuilder MapCommon(this WebApplication app) {

        var isDev = app.Environment.IsDevelopment();

        app.UseHttpsViaHsts(isDev);     // Enforce HSTS, so Https in non-development environments
        app.UseClientHeadersInProxy();  // Preserve client headers behind proxy,  ratelimiting and redirects
        app.UseRateLimiting();          // Apply global rate limiting middleware

        if (isDev) {
            // /openapi/{documentName}.json
            var openApi = app.MapOpenApi(); // Expose OpenAPI only in dev
            openApi.AllowAnonymous();       // Make OpenAPI spec accessible without auth
            openApi.DisableRateLimiting();  // Do not rate-limit Swagger/OpenAPI

            // Serve Swagger UI at /swagger and point it to the OpenAPI document
            app.UseSwaggerUI(c => {
                c.SwaggerEndpoint("/openapi/document.json", "WebApi");
                c.RoutePrefix = "swagger"; // UI at /swagger
            });
        }

        app.UseExceptionHandler(_ => { }); // Centralized exception handling -> ProblemDetails formatting
        app.UseHttpsRedirection();         // Redirect all HTTP -> HTTPS early
        app.UseCorrelationId();            // Inject/propagate correlation ID (X-Correlation-ID)
        app.UseSecurityHeaders(isDev); // CSP + security headers (strict in prod)
        app.UseCors("DefaultCors");      // Apply configured CORS policy
        app.UseHttpLogging();             // Request/response metadata logging for audit
        app.UseAuthentication();          // Validate JWT bearer tokens
        app.UseAuthorization();           // Enforce policies/fallback

        if (isDev) {
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
        // Exempt health checks from rate limiting
        live.DisableRateLimiting();
        ready.DisableRateLimiting();

        return app; // Return route builder for further chaining in wbHost Program
    }


    private static IServiceCollection AddHttpsViaHsts(this IServiceCollection services) {
        services.AddHsts(options => {
            options.MaxAge = TimeSpan.FromDays(365);
            options.IncludeSubDomains = true;
            options.Preload = true;
        }
        );

        return services;
    }
    private static IEndpointRouteBuilder UseHttpsViaHsts(this WebApplication app, bool isDev) {

        // Enforce HSTS (strict transport) in non-development environments
        if (!isDev)
            app.UseHsts();

        return app;
    }


    public static IServiceCollection AddClientHeadersInProxy(this IServiceCollection services) {


        services.Configure<ForwardedHeadersOptions>(options => {
            options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
            options.KnownNetworks.Clear(); // Explicit trust config handled at infrastructure level
            options.KnownProxies.Clear();  // Avoid implicit trusts 
            // NOTE: If required, you can add:
            // options.KnownProxies.Add(IPAddress.Parse("10.0.0.10"));
            // or:
            // options.KnownNetworks.Add(new IPNetwork(IPAddress.Parse("10.0.0.0"), 8));
        });
        return services;
    }
    private static IEndpointRouteBuilder UseClientHeadersInProxy(this WebApplication app) {
        // use before  auth, redirects,link generation midleware

        app.UseForwardedHeaders();      // Preserve client headers across proxy,  ratelimiting and redirects

        return app;
    }


    public static IServiceCollection AddRateLimiting(this IServiceCollection services) {
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
    private static IEndpointRouteBuilder UseRateLimiting(this WebApplication app) {
        app.UseRateLimiter();          // Apply global rate limiting middleware
         
        return app;
    } 


    private static IResult CreateDevToken(IConfiguration configuration) {
        // Extract token issuance parameters from config
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

    public static IServiceCollection AddSecureToken(this IServiceCollection services, IConfiguration configuration, IHostEnvironment env) {
        var audience = configuration["Auth:Audience"]!;
        var issuer = configuration["Auth:Issuer"]!;
        var devKey = configuration["Auth:DevKey"]!;

        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options => {
                options.MapInboundClaims = false;
                options.RequireHttpsMetadata = !env.IsDevelopment();
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

    private static IServiceCollection AddSecureHttpJson(this IServiceCollection services) {
        services.ConfigureHttpJsonOptions(o => {
            o.SerializerOptions.ReadCommentHandling = JsonCommentHandling.Disallow;
            o.SerializerOptions.MaxDepth = 32;
            o.SerializerOptions.PropertyNameCaseInsensitive = false;
        });
        return services;
    }

    public static IServiceCollection AddSecureBrowser(this IServiceCollection services, IConfiguration configuration) {
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

    public static IServiceCollection AddSecureHttpLogging(this IServiceCollection services) {
        services.AddHttpLogging(options => {
            options.LoggingFields =
                HttpLoggingFields.Request |
                HttpLoggingFields.Response |
                HttpLoggingFields.Duration;
        });
        return services;
    }



}