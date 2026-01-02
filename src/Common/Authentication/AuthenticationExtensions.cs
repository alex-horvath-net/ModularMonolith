using Common.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace Common.Authentication;

internal static class AuthenticationExtensions {

    public static IServiceCollection AddAuthentication(this IServiceCollection services, IConfiguration configuration, IHostEnvironment env) {

        // Configure authentication middleware; TokenValidationParameters will be set by the post-configure
        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options => {
                options.MapInboundClaims = false;
                options.RequireHttpsMetadata = !env.IsDevelopment();
            });

        // Ensure authorization services are present with a fallback policy
        services.AddAuthorization(options => {
            options.FallbackPolicy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
        });

        // Register the post-configure which runs against the real container after DI composition
        services.AddSingleton<IPostConfigureOptions<JwtBearerOptions>, JwtBearerPostConfigure>();

        // Register resolver and provider (resolver first)
        services.AddSingleton<ICertificateResolver, CertificateResolver>();
        services.AddSingleton<IJwtSigningCredentialProvider, JwtSigningCredentialProvider>();

        services.AddOptions<JwtOptions>()
            .Bind(configuration.GetSection(JwtOptions.SectionName))
            .ValidateOnStart();
        services.AddSingleton<IValidateOptions<JwtOptions>, JwtOptionsValidator>();


        services
            .AddAuthorizationBuilder();

        return services;
    }

    public static IEndpointRouteBuilder MapAuthentication(this WebApplication app) {

        app.UseAuthentication();        // Validate JWT bearer tokens
        app.UseAuthorization();         // Enforce policies/fallback

        return app;
    }
}