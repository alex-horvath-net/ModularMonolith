using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;

namespace Common;

public static class Extensions {
    public static IServiceCollection AddCommon(this IServiceCollection services, IConfiguration configuration) {

        var audience = configuration["Auth:Audience"]!;
        var issuer = configuration["Auth:Issuer"]!;
        var devKey = configuration["Auth:DevKey"]!;

        services.AddOpenApi();

        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options => {
                options.MapInboundClaims = false;
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters = new TokenValidationParameters {
                    ValidateIssuer = true,
                    ValidIssuer = issuer,
                    ValidateAudience = true,
                    ValidAudience = audience,
                    ValidateLifetime = true,
                    RequireExpirationTime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(devKey)),
                    ClockSkew = TimeSpan.FromSeconds(30),
                    ValidTypes = new[] { "JWT", "at+jwt" },
                    ValidAlgorithms = new[] { SecurityAlgorithms.HmacSha256 }
                };
            });


        return services.AddScoped<IBusinessEventPublisher, InProcessBusinessEventPublisher>();
    }

    public static IEndpointRouteBuilder MapCommon(this WebApplication app) {

        var configuration = app.Services.GetRequiredService<IConfiguration>();
        var audience = configuration["Auth:Audience"]!;
        var issuer = configuration["Auth:Issuer"]!;
        var devKey = configuration["Auth:DevKey"]!;


        if (app.Environment.IsDevelopment()) {
            app.MapOpenApi();
        }

        app.UseHttpsRedirection();
        app.UseAuthentication();
        app.UseAuthorization();


        if (app.Environment.IsDevelopment()) {
            var group = app.MapGroup("/auth").WithTags("Auth").AllowAnonymous();
            group.MapPost("/dev-token", CreateDevToken);
        }

        return app;
    }

    //private static IResult GetDevToken(string audience, string issuer, string devKey) {
    private static IResult CreateDevToken(IConfiguration configuration) {

        var audience = configuration["Auth:Audience"]!;
        var issuer = configuration["Auth:Issuer"]!;
        var devKey = configuration["Auth:DevKey"]!;
        var devScopes = configuration.GetSection("Auth:DevScopes").Get<string[]>() ?? Array.Empty<string>();  //  "DevScopes": [ "orders.read", "orders.write", "billing.read" ]

        var handler = new JwtSecurityTokenHandler();

        var claims = new List<Claim>();
        claims.Add(new Claim(JwtRegisteredClaimNames.Sub, "dev-user"));
        claims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString("N")));
        claims.Add(new Claim(JwtRegisteredClaimNames.Iat, EpochTime.GetIntDate(DateTime.UtcNow).ToString(), ClaimValueTypes.Integer64));
        claims.AddRange(devScopes.Select(devScope => new Claim("scope", devScope)));
        var subject = new ClaimsIdentity(claims);
        var signingCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(devKey)), SecurityAlgorithms.HmacSha256);

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
