namespace Common.Authentication;

internal sealed class JwtOptions {
    public static string SectionName => "Auth";
    public string? Issuer { get; set; }
    public string? Audience { get; set; }

    // Development symmetric key (must only be used in non-production and when explicitly allowed)
    public string? DevKey { get; set; }
    public bool AllowDevSymmetricKey { get; set; } = false;

    // Certificate-based signing options for production/HSM
    public bool UseCertificateForJwtSigning { get; set; } = false;
    public string? CertificateThumbprint { get; set; }
    public string? CertificateSubjectName { get; set; } // optional alternative to thumbprint
    public string? CertificatePath { get; set; } // optional PFX path
    public string? CertificatePassword { get; set; } // optional PFX password (prefer KeyVault)

    // Enforce online revocation checking by default
    public bool EnforceCertificateRevocation { get; set; } = true;
}
