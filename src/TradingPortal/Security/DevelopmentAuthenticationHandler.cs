using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace TradingPortal.Security;

public sealed class DevelopmentAuthenticationHandler(
    IOptionsMonitor<AuthenticationSchemeOptions> options,
    ILoggerFactory logger,
    UrlEncoder encoder,
    IConfiguration configuration)
    : AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder) {
    public const string SchemeName = "Development";

    protected override Task<AuthenticateResult> HandleAuthenticateAsync() {
        var section = configuration.GetSection("DevelopmentUser");

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, Required(section, "ExternalUserId")),
            new("sub", Required(section, "ExternalUserId")),
            new(ClaimTypes.Name, Required(section, "UserName")),
            new("name", Required(section, "DisplayName")),
            new(ClaimTypes.Email, Required(section, "Email")),
            new("email", Required(section, "Email")),
            new("desk", Required(section, "Desk"))
        };

        foreach (var role in section.GetSection("Roles").Get<string[]>() ?? []) {
            claims.Add(new Claim(ClaimTypes.Role, role));
            claims.Add(new Claim("roles", role));
        }

        foreach (var scope in section.GetSection("Scopes").Get<string[]>() ?? []) {
            claims.Add(new Claim("scope", scope));
        }

        var identity = new ClaimsIdentity(claims, SchemeName);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, SchemeName);

        return Task.FromResult(AuthenticateResult.Success(ticket));
    }

    private static string Required(IConfiguration configuration, string key) => configuration[key]
            ?? throw new InvalidOperationException($"Missing development user configuration: DevelopmentUser:{key}");
}