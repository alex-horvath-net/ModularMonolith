using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Asp.Versioning;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Common.Authentication;

public static class CreateTokenEndPoint {

    public static IEndpointRouteBuilder MapDevToken(this WebApplication app) {

        if (app.Environment.IsDevelopment()) {

            var version = app.NewApiVersionSet()
                .HasApiVersion(new ApiVersion(1, 0))
                .ReportApiVersions()
                .Build();

            var group = app.MapGroup("/v{version:apiVersion}/devtokens")
                .WithApiVersionSet(version)
                .WithTags("DevTokens");

            group.MapPost("/", CreateDevToken)
                .AllowAnonymous()
                .RequireRateLimiting("writes")
                .Produces(StatusCodes.Status201Created)
                .ProducesValidationProblem();
        }

        return app;
    }

    private static IResult CreateDevToken(IOptions<JwtOptions> options) {
        var claims = GetClaims(options.Value.DevScopes);
        var payload = new ClaimsIdentity(claims);
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(options.Value.SecurityKey!));
        var signature = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var tokenHandler = new JwtSecurityTokenHandler();

        var token = tokenHandler.CreateJwtSecurityToken(
            issuer: options.Value.Issuer,
            audience: options.Value.Audience,
            notBefore: DateTime.UtcNow,
            expires: DateTime.UtcNow.AddMinutes(10),
            subject: payload,
            signingCredentials: signature
        );

        var result = new {
            access_token = tokenHandler.WriteToken(token),
            token_type = "Bearer",
            expires_in = 600
        };

        return Results.Ok(result);
    }

    private static List<Claim> GetClaims(string[] scopes) {
        var claims = new List<Claim>();
        claims.Add(new(JwtRegisteredClaimNames.Sub, "dev-user"));
        claims.Add(new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString("N")));
        claims.Add(new(JwtRegisteredClaimNames.Iat, EpochTime.GetIntDate(DateTime.UtcNow).ToString(), ClaimValueTypes.Integer64));
        foreach (var scope in scopes) {
            claims.Add(new Claim("scope", scope));
        }

        return claims;
    }
}
