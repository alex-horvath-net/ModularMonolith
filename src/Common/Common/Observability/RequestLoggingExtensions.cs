using Microsoft.AspNetCore.HttpLogging;
using Microsoft.Extensions.DependencyInjection;

namespace Common.Observability {
    internal static class RequestLoggingExtensions {

        public static IServiceCollection AddRequestLogging(this IServiceCollection services) {
            // Rename from AddRequestogging; configuration preserved
            services.AddHttpLogging(o => {
                o.LoggingFields =
                    HttpLoggingFields.Request |
                    HttpLoggingFields.Response |
                    HttpLoggingFields.Duration |
                    HttpLoggingFields.RequestHeaders |
                    HttpLoggingFields.ResponseHeaders;
                o.RequestHeaders.Add("Host");
                o.RequestHeaders.Add("User-Agent");
                o.RequestHeaders.Add("X-Correlation-ID");
                o.ResponseHeaders.Add("Content-Type");
                o.ResponseHeaders.Add("Content-Length");
                // Do NOT add: Authorization, Cookie, Set-Cookie
            });
            return services;
        }
    }
}