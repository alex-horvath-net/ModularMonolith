using Core.Infrastructure.Authentication;
using Core.Infrastructure.Authentication.Basic;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Infrastructure.Authentication.Basic;

internal static class BasicAuthenticationExtensions {

    public static IServiceCollection AddAuthentication(this IServiceCollection services, IConfiguration configuration) {

        // Optional: fail fast if config is missing.
        var basicSection = configuration.GetSection(BasicAuthenticationOptions.SectionName);
        services.Configure<BasicAuthenticationOptions>(basicSection);

        // Register your credential validator.
        // In production this should validate against a user store, secret vault, hash, etc.
        services.AddSingleton<IBasicCredentialValidator, BasicCredentialValidator>();

        services
            .AddAuthentication(BasicAuthenticationDefaults.AuthenticationScheme)
            .AddScheme<BasicAuthenticationOptions, BasicAuthenticationHandler>(
                BasicAuthenticationDefaults.AuthenticationScheme,
                options => {
                    // Bind from configuration so the handler can read Realm.
                    basicSection.Bind(options);
                });

        services.AddAuthorization();

        return services;
    }

    public static IEndpointRouteBuilder MapAuthentication(this WebApplication app) {

        app.UseAuthentication();
        app.UseAuthorization();

        return app;
    }
}