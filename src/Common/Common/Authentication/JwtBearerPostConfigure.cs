using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Common.Authentication;

internal sealed class JwtBearerPostConfigure : IPostConfigureOptions<JwtBearerOptions> {
    private readonly JwtOptions _jwtOptions;
    private readonly IJwtSigningCredentialProvider _provider;
    private readonly IHostEnvironment _env;
    private readonly ILogger<JwtBearerPostConfigure> _logger;

    public JwtBearerPostConfigure(IOptions<JwtOptions> jwtOptions, IJwtSigningCredentialProvider provider, IHostEnvironment env, ILogger<JwtBearerPostConfigure> logger) {
        _jwtOptions = jwtOptions?.Value ?? throw new ArgumentNullException(nameof(jwtOptions));
        _provider = provider ?? throw new ArgumentNullException(nameof(provider));
        _env = env ?? throw new ArgumentNullException(nameof(env));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public void PostConfigure(string name, JwtBearerOptions options) {
        // Standard middleware settings
        options.MapInboundClaims = false;
        options.RequireHttpsMetadata = !_env.IsDevelopment();

        // Validate required auth options
        var issuer = _jwtOptions.Issuer ?? throw new InvalidOperationException("Auth:Issuer is not configured.");
        var audience = _jwtOptions.Audience ?? throw new InvalidOperationException("Auth:Audience is not configured.");

        // Resolve signing key via DI-backed provider (may throw on misconfiguration)
        var key = _provider.GetValidationKey();

        // Enforce production prohibition of symmetric keys
        if (_env.IsProduction() && key is SymmetricSecurityKey) {
            throw new InvalidOperationException("Symmetric signing keys must not be used in production. Configure an X.509 certificate or HSM-backed key for JWT signing.");
        }

        options.TokenValidationParameters = new TokenValidationParameters {
            ValidateIssuer = true,
            ValidIssuer = issuer,
            ValidateAudience = true,
            ValidAudience = audience,
            ValidateLifetime = true,
            RequireExpirationTime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = key,
            ClockSkew = TimeSpan.FromSeconds(30),
            ValidTypes = new[] { "JWT", "at+jwt" },
            ValidAlgorithms = new[] { SecurityAlgorithms.HmacSha256, SecurityAlgorithms.RsaSha256, SecurityAlgorithms.RsaSha512 }
        };

        // Instrument authentication pipeline with structured, non-sensitive logs.
        options.Events = new JwtBearerEvents {
            OnMessageReceived = context => {
                var correlationId = GetCorrelationId(context.HttpContext);
                var hasAuth = !string.IsNullOrEmpty(context.Request.Headers["Authorization"]);
                _logger.LogDebug("JwtBearer.OnMessageReceived; authHeaderPresent={AuthPresent}, correlationId={CorrelationId}, activityId={ActivityId}",
                    hasAuth, correlationId, System.Diagnostics.Activity.Current?.Id);
                return Task.CompletedTask;
            },

            OnTokenValidated = context => {
                var correlationId = GetCorrelationId(context.HttpContext);
                var sub = context.Principal?.FindFirst("sub")?.Value;
                var subHash = sub is null ? null : HashIdentifier(sub);
                _logger.LogInformation("JwtBearer.OnTokenValidated; subHash={SubHash}, issuer={Issuer}, audience={Audience}, correlationId={CorrelationId}, activityId={ActivityId}",
                    subHash, _jwtOptions.Issuer, _jwtOptions.Audience, correlationId, System.Diagnostics.Activity.Current?.Id);
                return Task.CompletedTask;
            },

            OnAuthenticationFailed = context => {
                var correlationId = GetCorrelationId(context.HttpContext);
                _logger.LogWarning(context.Exception, "JwtBearer.OnAuthenticationFailed; message={Message}, correlationId={CorrelationId}, activityId={ActivityId}",
                    context.Exception.Message, correlationId, System.Diagnostics.Activity.Current?.Id);
                return Task.CompletedTask;
            },

            OnChallenge = context => {
                var correlationId = GetCorrelationId(context.HttpContext);
                _logger.LogWarning("JwtBearer.OnChallenge; error={Error}, description={Description}, correlationId={CorrelationId}, activityId={ActivityId}",
                    context.Error, context.ErrorDescription, correlationId, System.Diagnostics.Activity.Current?.Id);
                return Task.CompletedTask;
            },

            OnForbidden = context => {
                var correlationId = GetCorrelationId(context.HttpContext);
                _logger.LogWarning("JwtBearer.OnForbidden; path={Path}, correlationId={CorrelationId}, activityId={ActivityId}",
                    context.HttpContext?.Request?.Path, correlationId, System.Diagnostics.Activity.Current?.Id);
                return Task.CompletedTask;
            }
        };
    }

    private static string? GetCorrelationId(Microsoft.AspNetCore.Http.HttpContext? ctx) {
        if (ctx == null) return null;
        if (ctx.Request.Headers.TryGetValue("X-Correlation-ID", out var v) && !string.IsNullOrWhiteSpace(v)) return v.ToString();
        if (ctx.Request.Headers.TryGetValue("Correlation-Id", out v) && !string.IsNullOrWhiteSpace(v)) return v.ToString();
        return System.Diagnostics.Activity.Current?.Id;
    }

    private static string HashIdentifier(string id) {
        // SHA256 and return short hex prefix to avoid PII
        using var sha = SHA256.Create();
        var hash = sha.ComputeHash(Encoding.UTF8.GetBytes(id));
        return BitConverter.ToString(hash, 0, 4).Replace("-", "").ToLowerInvariant(); // first 8 hex chars
    }
}
