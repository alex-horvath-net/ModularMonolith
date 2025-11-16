using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace Common;

public static class CommonExtensions {
    public static IServiceCollection AddCommon(this IServiceCollection services, IConfiguration configuration) {

        var audience = configuration["Auth:Audience"]!;
        var issuer = configuration["Auth:Issuer"]!;
        var devKey = configuration["Auth:DevKey"]!;

        services.AddOpenApi();

        // RFC7807 problem details
        services.AddProblemDetails();

        // CORS: secure by default, allow configured origins
        services.AddCors(options => {
            options.AddPolicy("DefaultCors", policy => {
                var origins = configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? Array.Empty<string>();
                if (origins.Length > 0) {
                    policy.WithOrigins(origins).AllowAnyHeader().AllowAnyMethod();
                }
            });
        });

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

        // Default deny unless explicitly allowed
        services
            .AddAuthorizationBuilder()
            .SetFallbackPolicy(new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build());

        // HTTP logging for request/response metadata
        services.AddHttpLogging(options => {
            options.LoggingFields = HttpLoggingFields.RequestPropertiesAndHeaders
                                  | HttpLoggingFields.ResponsePropertiesAndHeaders;
        });


        // Rate limiting (fixed window)
        services.AddRateLimiter(_ =>
            _.AddFixedWindowLimiter("fixed", options => {
                options.PermitLimit = 100;
                options.Window = TimeSpan.FromSeconds(10);
                options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                options.QueueLimit = 50;
            }));

        // Add health checks
        services.AddHealthChecks()
            .AddCheck("self", () => HealthCheckResult.Healthy());


        services.AddScoped<IBusinessEventPublisher, InProcessBusinessEventPublisher>();

        return services;
    }

    public static IEndpointRouteBuilder MapCommon(this WebApplication app) {

        var configuration = app.Services.GetRequiredService<IConfiguration>();

        if (!app.Environment.IsDevelopment()) {
            app.UseHsts();
        }

        app.UseRateLimiter();


        if (app.Environment.IsDevelopment()) {
            app.MapOpenApi();
        }

        // Global exception handler + RFC7807 output
        app.UseExceptionHandler(options => { });

        app.UseHttpsRedirection();

        // Correlation ID and Security headers early in pipeline
        app.UseCorrelationId();
        app.UseSecurityHeaders(app.Environment.IsDevelopment());

        // CORS
        app.UseCors("DefaultCors");

        app.UseHttpLogging();

        app.UseAuthentication();
        app.UseAuthorization();

        if (app.Environment.IsDevelopment()) {
            var group = app.MapGroup("/auth").WithTags("Auth").AllowAnonymous();
            group.MapPost("/dev-token", CreateDevToken);
        }


        // Health endpoints (public)
        app.MapHealthChecks("/health/live").AllowAnonymous();
        app.MapHealthChecks("/health/ready").AllowAnonymous();


        return app;
    }

    private static IResult CreateDevToken(IConfiguration configuration) {
        var audience = configuration["Auth:Audience"]!;
        var issuer = configuration["Auth:Issuer"]!;
        var devKey = configuration["Auth:DevKey"]!;
        var devScopes = configuration.GetSection("Auth:DevScopes").Get<string[]>() ?? Array.Empty<string>();

        var handler = new JwtSecurityTokenHandler();

        var claims = new List<Claim> {
            new(JwtRegisteredClaimNames.Sub, "dev-user"),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString("N")),
            new(JwtRegisteredClaimNames.Iat, EpochTime.GetIntDate(DateTime.UtcNow).ToString(), ClaimValueTypes.Integer64)
        };
        claims.AddRange(devScopes.Select(devScope => new Claim("scope", devScope)));
        var subject = new ClaimsIdentity(claims);
        var signingCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(devKey)), SecurityAlgorithms.HmacSha256);

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
}
