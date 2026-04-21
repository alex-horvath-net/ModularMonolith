using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Core.Infrastructure.Authentication.Basic;

public sealed class BasicAuthenticationHandler(
    IOptionsMonitor<BasicAuthenticationOptions> options,
    ILoggerFactory logger,
    UrlEncoder encoder,
    IBasicCredentialValidator credentialValidator) : AuthenticationHandler<BasicAuthenticationOptions>(options, logger, encoder) {

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync() {
        // No Authorization header -> no result.
        // This lets [Authorize] trigger the challenge flow.
        if (!Request.Headers.TryGetValue("Authorization", out var authorizationHeaderValues)) {
            return AuthenticateResult.NoResult();
        }

        var authorizationHeader = authorizationHeaderValues.ToString();

        if (string.IsNullOrWhiteSpace(authorizationHeader)) {
            return AuthenticateResult.NoResult();
        }

        AuthenticationHeaderValue? headerValue;

        try {
            headerValue = AuthenticationHeaderValue.Parse(authorizationHeader);
        } catch (FormatException) {
            return AuthenticateResult.Fail("Invalid Authorization header format.");
        }

        if (!BasicAuthenticationDefaults.AuthenticationScheme.Equals(
                headerValue.Scheme,
                StringComparison.OrdinalIgnoreCase)) {
            return AuthenticateResult.NoResult();
        }

        if (string.IsNullOrWhiteSpace(headerValue.Parameter)) {
            return AuthenticateResult.Fail("Missing Basic credentials.");
        }

        string userName;
        string password;

        try {
            // Basic <base64(username:password)>
            var credentialBytes = Convert.FromBase64String(headerValue.Parameter);
            var decoded = Encoding.UTF8.GetString(credentialBytes);

            var separatorIndex = decoded.IndexOf(':');
            if (separatorIndex <= 0) {
                return AuthenticateResult.Fail("Invalid Basic credential payload.");
            }

            userName = decoded[..separatorIndex];
            password = decoded[(separatorIndex + 1)..];
        } catch (FormatException) {
            return AuthenticateResult.Fail("Basic credentials are not valid Base64.");
        }

        var validationResult = await credentialValidator.ValidateAsync(userName, password, Context.RequestAborted);

        if (!validationResult.IsAuthenticated) {
            return AuthenticateResult.Fail("Invalid username or password.");
        }

        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, validationResult.UserName)
        };

        foreach (var role in validationResult.Roles) {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var identity = new ClaimsIdentity(
            claims,
            BasicAuthenticationDefaults.AuthenticationScheme);

        var principal = new ClaimsPrincipal(identity);

        var ticket = new AuthenticationTicket(
            principal,
            BasicAuthenticationDefaults.AuthenticationScheme);

        return AuthenticateResult.Success(ticket);
    }

    protected override Task HandleChallengeAsync(AuthenticationProperties properties) {
        Response.StatusCode = StatusCodes.Status401Unauthorized;

        // Standard Basic auth challenge header.
        Response.Headers.WWWAuthenticate = $"Basic realm=\"{Options.Realm}\", charset=\"UTF-8\"";

        return Task.CompletedTask;
    }
}
