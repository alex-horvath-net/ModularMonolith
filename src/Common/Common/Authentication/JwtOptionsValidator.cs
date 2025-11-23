using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace Common.Authentication;

internal sealed class JwtOptionsValidator : IValidateOptions<JwtOptions> {
    private readonly IHostEnvironment _env;

    public JwtOptionsValidator(IHostEnvironment env) {
        _env = env;
    }

    public ValidateOptionsResult Validate(string name, JwtOptions options) {
        if (options == null)
            return ValidateOptionsResult.Fail("JwtOptions is null.");

        if (string.IsNullOrWhiteSpace(options.Issuer))
            return ValidateOptionsResult.Fail("Auth:Issuer must be configured.");

        if (string.IsNullOrWhiteSpace(options.Audience))
            return ValidateOptionsResult.Fail("Auth:Audience must be configured.");

        if (_env.IsProduction()) {
            if (!options.UseCertificateForJwtSigning)
                return ValidateOptionsResult.Fail("In Production, Auth:UseCertificateForJwtSigning must be true.");

            if (string.IsNullOrWhiteSpace(options.CertificateThumbprint) && string.IsNullOrWhiteSpace(options.CertificatePath))
                return ValidateOptionsResult.Fail("In Production, configure Auth:CertificateThumbprint or Auth:CertificatePath to a PFX.");

            if (options.AllowDevSymmetricKey)
                return ValidateOptionsResult.Fail("Auth:AllowDevSymmetricKey must be false in Production.");
        }

        return ValidateOptionsResult.Success;
    }
}
