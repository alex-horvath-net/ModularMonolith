using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Common {
    internal static class IncomingHttpRequestsFromBrowserExtensions {

        internal static IServiceCollection AddBrowserRequestRestrictions(this IServiceCollection services, IConfiguration configuration) {

            var origins = configuration
                .GetSection("Cors:AllowedOrigins")
                .Get<string[]>() ??
                Array.Empty<string>();

            services.AddCors(options => {
                options.AddPolicy("DefaultCors", policy => {
                    if (origins.Length > 0) {
                        policy
                            .WithOrigins(origins)
                            .AllowAnyHeader()
                            .AllowAnyMethod();
                    }
                });
            });
            return services;
        }

        internal static WebApplication UseBrowserRequestRestrictions(this WebApplication app) {
            app.UseCors("DefaultCors");       // Apply configured CORS policy

            return app;
        }
    }
}