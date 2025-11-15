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

public static class BusinessEventExtensions {
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
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(devKey))
                };
            });


        return services.AddScoped<IBusinessEventPublisher, InProcessEventBus>();
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


        // All auth-related endpoints under /auth
        var group = app.MapGroup("/auth").WithTags("Auth").AllowAnonymous();

        // FIX: Map under /auth prefix and capture config values
        group.MapPost("/dev-token", CreateDevToken);

        return app;
    }

    //private static IResult GetDevToken(string audience, string issuer, string devKey) {
    private static IResult CreateDevToken(IConfiguration configuration) {

        var audience = configuration["Auth:Audience"]!;
        var issuer = configuration["Auth:Issuer"]!;
        var devKey = configuration["Auth:DevKey"]!;

        var handler = new JwtSecurityTokenHandler();

        var claims = new List<Claim> {
            new("sub", "dev-user"),
            new("scope", "orders.read"),
            new("scope", "orders.write"),
            new("scope", "billing.read")
        };

        var token = handler.CreateJwtSecurityToken(
            issuer: issuer,
            audience: audience,
            subject: new ClaimsIdentity(claims),
            notBefore: DateTime.UtcNow,
            expires: DateTime.UtcNow.AddMinutes(60),
            signingCredentials: new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(devKey)), SecurityAlgorithms.HmacSha256)
        );

        return Results.Ok(new { access_token = handler.WriteToken(token), token_type = "Bearer", expires_in = 3600 });
    }
}
