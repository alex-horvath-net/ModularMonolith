using System.Text;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Common.Authentication;

internal interface IJwtSigningCredentialProvider {
    public SecurityKey GetValidationKey();
}


internal sealed class JwtSigningCredentialProvider : IJwtSigningCredentialProvider {
    private readonly IOptionsMonitor<JwtOptions> _optionsMonitor;
    private readonly IHostEnvironment _env;
    private readonly ICertificateResolver _certResolver;
    private readonly ILogger<JwtSigningCredentialProvider> _logger;

    // Cached key and sync primitives for thread-safe lazy initialization
    private volatile SecurityKey? _cachedKey;
    private readonly object _sync = new();

    public JwtSigningCredentialProvider(IOptionsMonitor<JwtOptions> optionsMonitor, IHostEnvironment env, ICertificateResolver certResolver, ILogger<JwtSigningCredentialProvider> logger) {
        _optionsMonitor = optionsMonitor ?? throw new ArgumentNullException(nameof(optionsMonitor));
        _env = env ?? throw new ArgumentNullException(nameof(env));
        _certResolver = certResolver ?? throw new ArgumentNullException(nameof(certResolver));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        // Invalidate cache when options change (supports rotation without restart)
        _optionsMonitor.OnChange(_ => {
            Invalidate();
            _logger.LogInformation("JwtOptions changed — invalidated cached signing key.");
        });
    }

    public SecurityKey GetValidationKey() {
        // Fast path: return cached key if present
        var key = _cachedKey;
        if (key != null) {
            _logger.LogDebug("Returning cached JWT validation key.");
            return key;
        }

        // Slow path: compute and cache
        lock (_sync) {
            if (_cachedKey != null) {
                _logger.LogDebug("Returning cached JWT validation key after lock.");
                return _cachedKey;
            }

            var options = _optionsMonitor.CurrentValue ?? throw new InvalidOperationException("Auth options are not configured.");
            ValidateRequiredOptions(options);

            var mustUseCertificate = _env.IsProduction() || options.UseCertificateForJwtSigning;

            try {
                if (mustUseCertificate) {
                    var cert = _certResolver.ResolveCertificate(options);
                    if (cert != null) {
                        _cachedKey = new X509SecurityKey(cert);
                        _logger.LogInformation("Loaded X509 signing key for JWT validation (certificate).");
                        return _cachedKey;
                    }

                    _logger.LogError("No certificate could be resolved but a certificate is required for JWT validation.");
                    throw new InvalidOperationException("Production JWT validation requires a certificate. Configure Authentication:CertificateThumbprint or Authentication:CertificatePath (PFX) and ensure the certificate is accessible (Key Vault / LocalMachine).");
                }

                if (options.AllowDevSymmetricKey) {
                    _cachedKey = CreateSymmetricKey(options.DevKey);
                    _logger.LogInformation("Loaded symmetric signing key for JWT validation (development key).");
                    return _cachedKey;
                }

                // If certificate is optional but configured, try one last time
                if (options.UseCertificateForJwtSigning) {
                    var cert = _certResolver.ResolveCertificate(options);
                    if (cert != null) {
                        _cachedKey = new X509SecurityKey(cert);
                        _logger.LogInformation("Loaded X509 signing key for JWT validation (optional certificate path).");
                        return _cachedKey;
                    }
                }

                _logger.LogError("No valid JWT signing key configuration found.");
                throw new InvalidOperationException("No valid JWT signing key configuration found. For non-production: set Authentication:AllowDevSymmetricKey=true with Authentication:DevKey (in user-secrets) or configure certificate-based signing.");
            }
            catch {
                // On failure ensure we don't leave a partially initialized cached key
                _cachedKey = null;
                throw;
            }
        }
    }

    private void Invalidate() {
        lock (_sync) {
            _cachedKey = null;
        }
    }

    private static void ValidateRequiredOptions(JwtOptions options) {
        if (string.IsNullOrWhiteSpace(options.Issuer))
            throw new InvalidOperationException("Authentication:Issuer is not configured.");

        if (string.IsNullOrWhiteSpace(options.Audience))
            throw new InvalidOperationException("Authentication:Audience is not configured.");
    }

    private static SecurityKey CreateSymmetricKey(string? devKey) {
        if (string.IsNullOrWhiteSpace(devKey))
            throw new InvalidOperationException("Authentication:AllowDevSymmetricKey is true but Authentication:DevKey is empty.");

        // Enforce minimum entropy: require 32 characters (256-bit) for symmetric dev keys
        if (devKey.Length < 32)
            throw new InvalidOperationException("Authentication:DevKey must be at least 32 characters for acceptable entropy.");

        var bytes = Encoding.UTF8.GetBytes(devKey);
        return new SymmetricSecurityKey(bytes);
    }
}
