using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Common.Authentication;

public static class DevTokenExtensions {

    public static IServiceCollection AddDevToken(this IServiceCollection services, IConfiguration configuration, IHostEnvironment env) {


        return services;
    }


    public static IEndpointRouteBuilder MapDevToken(this WebApplication app) {

        if (app.Environment.IsDevelopment()) {

            var group = app.MapGroup("/devtokens").WithTags("DevToken").AllowAnonymous(); // Dev-only auth helper
            group.MapPost("/", CreateDevToken);                         // Issue short-lived dev token
        }

        return app;
    }

    private static IResult CreateDevToken(IConfiguration configuration) {
        // Extract token issuance parameters from config
        var audience = configuration["Authentication:Audience"]!;
        var issuer = configuration["Authentication:Issuer"]!;
        var devKey = configuration["Authentication:DevKey"]!;
        var devScopes = configuration.GetSection("Authentication:DevScopes").Get<string[]>() ?? Array.Empty<string>(); // Scoped permissions for dev token

        var handler = new JwtSecurityTokenHandler();

        // Standard claims set for minimal bearer token
        var claims = new List<Claim> {
            new(JwtRegisteredClaimNames.Sub, "dev-user"),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString("N")),
            new(JwtRegisteredClaimNames.Iat, EpochTime.GetIntDate(DateTime.UtcNow).ToString(), ClaimValueTypes.Integer64)
        };
        claims.AddRange(devScopes.Select(devScope => new Claim("scope", devScope)));
        var subject = new ClaimsIdentity(claims);
        var signingCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(devKey)), SecurityAlgorithms.HmacSha256);

        // Create short-lived access token
        var token = handler.CreateJwtSecurityToken(
            issuer: issuer,
            audience: audience,
            subject: subject,
            notBefore: DateTime.UtcNow,
            expires: DateTime.UtcNow.AddMinutes(10),
            signingCredentials: signingCredentials
        );

        return Results.Ok(new { access_token = handler.WriteToken(token), token_type = "Bearer", expires_in = 600 });
    }

}
