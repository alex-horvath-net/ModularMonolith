using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Extensions.Logging;

namespace Common.Authentication;

internal sealed class CertificateResolver : ICertificateResolver {
    private readonly ILogger<CertificateResolver> _logger;

    public CertificateResolver(ILogger<CertificateResolver> logger) {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public X509Certificate2? ResolveCertificate(JwtOptions options) {
        if (options == null) {
            _logger.LogDebug("ResolveCertificate called with null options.");
            return null;
        }

        // 1) Thumbprint in LocalMachine (preferred)
        if (!string.IsNullOrWhiteSpace(options.CertificateThumbprint)) {
            try {
                var thumb = options.CertificateThumbprint.Replace(" ", string.Empty);
                using var store = new X509Store(StoreLocation.LocalMachine);
                store.Open(OpenFlags.ReadOnly);
                var found = store.Certificates.Find(X509FindType.FindByThumbprint, thumb, validOnly: false);
                var cert = found.OfType<X509Certificate2>().FirstOrDefault();
                if (cert != null) {
                    if (IsValidForJwtValidation(cert, options, out var reason)) {
                        _logger.LogInformation("Resolved certificate by thumbprint {Thumbprint}, expires {NotAfter:u}", cert.Thumbprint, cert.NotAfter.ToUniversalTime());
                        return cert;
                    }

                    _logger.LogWarning("Certificate found by thumbprint {Thumbprint} rejected: {Reason}", cert.Thumbprint, reason);
                }
            } catch (Exception ex) {
                _logger.LogDebug(ex, "Certificate lookup by thumbprint failed.");
            }
        }

        // 2) Subject distinguished name in LocalMachine
        if (!string.IsNullOrWhiteSpace(options.CertificateSubjectName)) {
            try {
                using var store = new X509Store(StoreLocation.LocalMachine);
                store.Open(OpenFlags.ReadOnly);
                var found = store.Certificates.Find(X509FindType.FindBySubjectDistinguishedName, options.CertificateSubjectName, validOnly: false);
                var cert = found.OfType<X509Certificate2>().FirstOrDefault();
                if (cert != null) {
                    if (IsValidForJwtValidation(cert, options, out var reason)) {
                        _logger.LogInformation("Resolved certificate by subject {Subject}, thumbprint {Thumbprint}, expires {NotAfter:u}", cert.Subject, cert.Thumbprint, cert.NotAfter.ToUniversalTime());
                        return cert;
                    }

                    _logger.LogWarning("Certificate found by subject {Subject} rejected: {Reason}", cert.Subject, reason);
                }
            } catch (Exception ex) {
                _logger.LogDebug(ex, "Certificate lookup by subject failed.");
            }
        }

        // 3) PFX path fallback (local file) — prefer KeyVault in production
        if (!string.IsNullOrWhiteSpace(options.CertificatePath)) {
            try {
                var cert = new X509Certificate2(options.CertificatePath, options.CertificatePassword, X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.PersistKeySet);
                if (cert != null) {
                    if (IsValidForJwtValidation(cert, options, out var reason)) {
                        _logger.LogInformation("Loaded certificate from PFX {Path}, thumbprint {Thumbprint}, expires {NotAfter:u}", options.CertificatePath, cert.Thumbprint, cert.NotAfter.ToUniversalTime());
                        return cert;
                    }

                    _logger.LogWarning("Certificate loaded from PFX {Path} rejected: {Reason}", options.CertificatePath, reason);
                }
            } catch (Exception ex) {
                _logger.LogDebug(ex, "Loading certificate from PFX path failed.");
            }
        }

        _logger.LogDebug("No certificate could be resolved from configured options.");
        return null;
    }

    private bool IsValidForJwtValidation(X509Certificate2 cert, JwtOptions options, out string reason) {
        reason = string.Empty;
        var now = DateTime.UtcNow;

        try {
            if (cert == null) {
                reason = "Certificate is null";
                return false;
            }

            // Date validity
            if (cert.NotBefore.ToUniversalTime() > now) {
                reason = $"Certificate not valid until {cert.NotBefore:u}";
                return false;
            }

            if (cert.NotAfter.ToUniversalTime() < now) {
                reason = $"Certificate expired at {cert.NotAfter:u}";
                return false;
            }

            // Public key algorithm check (accept RSA and ECDSA)
            var pkOid = cert.PublicKey.Oid?.Value ?? string.Empty;
            var acceptPk = pkOid == "1.2.840.113549.1.1.1" /* RSA */ || pkOid == "1.2.840.10045.2.1" /* ECC */;
            if (!acceptPk) {
                reason = $"Unsupported public key algorithm OID '{pkOid}'";
                return false;
            }

            // Key usage (if present) should include DigitalSignature for signing/verification
            var kuExt = cert.Extensions.OfType<X509KeyUsageExtension>().FirstOrDefault();
            if (kuExt != null) {
                if ((kuExt.KeyUsages & X509KeyUsageFlags.DigitalSignature) == 0) {
                    reason = "KeyUsage extension present but does not include DigitalSignature";
                    return false;
                }
            }

            // Enhanced Key Usage (if present) should include ServerAuth or ClientAuth (or be absent)
            var ekuExt = cert.Extensions.OfType<X509EnhancedKeyUsageExtension>().FirstOrDefault();
            if (ekuExt != null) {
                var usages = ekuExt.EnhancedKeyUsages.OfType<Oid>().Select(o => o.Value).ToArray();
                var allowed = new[] { "1.3.6.1.5.5.7.3.1", /* ServerAuth */ "1.3.6.1.5.5.7.3.2" /* ClientAuth */ };
                if (!usages.Any(u => allowed.Contains(u, StringComparer.OrdinalIgnoreCase))) {
                    reason = $"EnhancedKeyUsage present but does not include ServerAuth/ClientAuth (found: {string.Join(',', usages)})";
                    return false;
                }
            }

            // Public key presence
            if (cert.PublicKey == null || cert.PublicKey.EncodedKeyValue == null || cert.PublicKey.EncodedKeyValue.RawData == null || cert.PublicKey.EncodedKeyValue.RawData.Length == 0) {
                reason = "Certificate does not contain a usable public key";
                return false;
            }

            // Certificate chain + revocation checking (configurable)
            if (options.EnforceCertificateRevocation) {
                try {
                    using var chain = new X509Chain();
                    chain.ChainPolicy.RevocationMode = X509RevocationMode.Online;
                    chain.ChainPolicy.RevocationFlag = X509RevocationFlag.EntireChain;
                    chain.ChainPolicy.UrlRetrievalTimeout = TimeSpan.FromSeconds(5);
                    chain.ChainPolicy.VerificationFlags = X509VerificationFlags.NoFlag;

                    var buildOk = chain.Build(cert);

                    // If build failed, inspect statuses and reject (conservative)
                    if (!buildOk) {
                        var statuses = chain.ChainStatus ?? Array.Empty<X509ChainStatus>();
                        var statusMessages = string.Join("; ", statuses.Select(s => s.Status.ToString() + ":" + (s.StatusInformation ?? string.Empty).Trim()));
                        // If any revoked flag present, treat as revoked
                        if (statuses.Any(s => (s.Status & X509ChainStatusFlags.Revoked) != 0)) {
                            reason = $"Certificate chain indicates revocation ({statusMessages})";
                            _logger.LogWarning("Certificate {Thumbprint} rejected due to revocation status. Expires {NotAfter:u}", cert.Thumbprint, cert.NotAfter.ToUniversalTime());
                            _logger.LogDebug("Chain statuses: {ChainStatuses}", statusMessages);
                            return false;
                        }

                        // For any other chain errors, reject conservatively when revocation enforcement is enabled
                        reason = $"Certificate chain build failed ({statusMessages})";
                        _logger.LogWarning("Certificate {Thumbprint} rejected due to chain build failure: {Reason}", cert.Thumbprint, statusMessages);
                        _logger.LogDebug("Chain details: {ChainStatuses}", statusMessages);
                        return false;
                    }
                } catch (Exception ex) {
                    // Network or unexpected errors while checking revocation — be conservative and reject, but only expose debug details
                    _logger.LogDebug(ex, "Unexpected error while building certificate chain for {Thumbprint}", cert.Thumbprint);
                    reason = "Unexpected error while validating certificate chain";
                    _logger.LogWarning("Certificate {Thumbprint} rejected due to chain validation error", cert.Thumbprint);
                    return false;
                }
            }

            return true;
        } catch (Exception ex) {
            // Be conservative: reject on unexpected inspection errors but surface debug logs
            _logger.LogDebug(ex, "Unexpected error while validating certificate {Thumbprint}", cert?.Thumbprint);
            reason = "Unexpected error while validating certificate";
            return false;
        }
    }
}
