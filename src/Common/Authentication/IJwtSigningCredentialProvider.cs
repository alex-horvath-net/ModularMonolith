using Microsoft.IdentityModel.Tokens;

namespace Common.Authentication;

internal interface IJwtSigningCredentialProvider {
    public SecurityKey GetValidationKey();
}
