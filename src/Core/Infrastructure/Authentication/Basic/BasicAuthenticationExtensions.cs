using Core.Infrastructure.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Infrastructure.Authentication.Basic;

internal static class BasicAuthenticationExtensions {

    public static IServiceCollection AddBasicAuthentication(this IServiceCollection services, IConfiguration configuration) {

        var basicSection = configuration.GetSection(BasicAuthenticationOptions.SectionName);
        services.Configure<BasicAuthenticationOptions>(basicSection);

        services.AddSingleton<IBasicCredentialValidator, BasicCredentialValidator>();

        services.AddAuthentication(BasicAuthenticationDefaults.AuthenticationScheme)
                .AddScheme<BasicAuthenticationOptions, BasicAuthenticationHandler>(BasicAuthenticationDefaults.AuthenticationScheme, _ => { });

        services.AddAuthorization();

        return services;
    }

    public static WebApplication UseBasicAuthentication(this WebApplication app) {

        app.UseAuthentication();
        app.UseAuthorization();

        return app;
    }
}