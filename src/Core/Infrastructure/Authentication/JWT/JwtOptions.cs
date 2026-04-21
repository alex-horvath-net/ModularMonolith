using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace Core.Infrastructure.Authentication.JWT;

public sealed class JwtOptions {
    public static string SectionName => "Authentication";
    public string? Audience { get; set; }
    public string? Issuer { get; set; }

    // Development symmetric key (must only be used in non-production and when explicitly allowed)
    public string? SecurityKey { get; set; }
    public bool AllowDevSymmetricKey { get; set; }

    public string[] DevScopes { get; set; } = [];
    // Certificate-based signing options for production/HSM
    public bool UseCertificateForJwtSigning { get; set; }
    public string? CertificateThumbprint { get; set; }
    public string? CertificateSubjectName { get; set; } // optional alternative to thumbprint
    public string? CertificatePath { get; set; } // optional PFX path
    public string? CertificatePassword { get; set; } // optional PFX password (prefer KeyVault)

    // Enforce online revocation checking by default
    public bool EnforceCertificateRevocation { get; set; } = true;
}

internal sealed class JwtOptionsValidator(IHostEnvironment env, ILogger<JwtOptionsValidator> logger) : IValidateOptions<JwtOptions> {
    private readonly IHostEnvironment env = env ?? throw new ArgumentNullException(nameof(env));
    private readonly ILogger<JwtOptionsValidator> logger = logger ?? throw new ArgumentNullException(nameof(logger));

    public ValidateOptionsResult Validate(string? name, JwtOptions options) {
        logger.LogInformation($"{nameof(Validate)} is called");

        if (options == null)
            return ValidateOptionsResult.Fail("JwtOptions is null.");

        if (string.IsNullOrWhiteSpace(options.Issuer))
            return ValidateOptionsResult.Fail("Authentication:Issuer must be configured.");

        if (string.IsNullOrWhiteSpace(options.Audience))
            return ValidateOptionsResult.Fail("Authentication:Audience must be configured.");

        if (env.IsProduction()) {
            if (!options.UseCertificateForJwtSigning)
                return ValidateOptionsResult.Fail("In Production, Authentication:UseCertificateForJwtSigning must be true.");

            if (string.IsNullOrWhiteSpace(options.CertificateThumbprint) && string.IsNullOrWhiteSpace(options.CertificatePath))
                return ValidateOptionsResult.Fail("In Production, configure Authentication:CertificateThumbprint or Authentication:CertificatePath to a PFX.");

            if (options.AllowDevSymmetricKey)
                return ValidateOptionsResult.Fail("Authentication:AllowDevSymmetricKey must be false in Production.");
        }

        return ValidateOptionsResult.Success;
    }
}
