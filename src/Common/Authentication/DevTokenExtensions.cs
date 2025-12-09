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

    public static IServiceCollection AddDevToken(this IServiceCollection services) {


        return services;
    }


    public static IEndpointRouteBuilder MapDevToken(this WebApplication app) {

        if (app.Environment.IsDevelopment()) {

            var group = app.MapGroup("/devtokens").WithTags("DevToken").AllowAnonymous(); // Dev-only auth helper
            group.MapPost("/", CreateDevToken);                         // Issue short-lived dev token
        }

        return app;
    }

    private static IResult CreateDevToken(IOptions<JwtOptions> options) {
        var claims = GetClaims(options.Value.DevScopes);
        var payload = new ClaimsIdentity(claims);
        var signature = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(options.Value.DevKey!)), SecurityAlgorithms.HmacSha256);
        
        // Create short-lived access token
        var handler = new JwtSecurityTokenHandler();


        var token = handler.CreateJwtSecurityToken(
            issuer: options.Value.Issuer,
            audience: options.Value.Audience,
            notBefore: DateTime.UtcNow,
            expires: DateTime.UtcNow.AddMinutes(10),
            subject: payload,
            signingCredentials: signature
        );
        
        var access_token = handler.WriteToken(token);
       
        var result = new {
            access_token,
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
