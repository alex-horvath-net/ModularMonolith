using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Common.Security;

internal static class HttpsExtensioins{

    internal static IServiceCollection AddHttps(this IServiceCollection services) {
        services.AddHsts(options => {
            options.MaxAge = TimeSpan.FromDays(365);
            options.IncludeSubDomains = true;
            options.Preload = true;
        }
        );

        return services;
    }
    internal static IEndpointRouteBuilder UseHttps(this WebApplication app) {

        // Enforce HSTS (strict transport) in non-development environments
        if (!app.Environment.IsDevelopment())
            app.UseHsts();

        return app;
    }
}