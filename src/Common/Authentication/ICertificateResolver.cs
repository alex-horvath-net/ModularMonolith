using System.Security.Cryptography.X509Certificates;

namespace Common.Authentication;

internal interface ICertificateResolver {
    /// <summary>
    /// Resolves an X509 certificate according to configured options.
    /// Returns null when no certificate can be resolved (caller decides how to handle).
    /// </summary>
    X509Certificate2? ResolveCertificate(JwtOptions options);
}
