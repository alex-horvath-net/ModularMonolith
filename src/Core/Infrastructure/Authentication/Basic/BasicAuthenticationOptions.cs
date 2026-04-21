using Microsoft.AspNetCore.Authentication;

namespace Core.Infrastructure.Authentication.Basic;

public sealed class BasicAuthenticationOptions : AuthenticationSchemeOptions {
    public const string SectionName = "BasicAuthentication";

    // Used in the WWW-Authenticate challenge header:
    // WWW-Authenticate: Basic realm="My API"
    public string Realm { get; set; } = "My API";
}