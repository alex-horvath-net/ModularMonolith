using Asp.Versioning;
using Microsoft.Extensions.DependencyInjection;

namespace Common.Version;
public static class ApiVersioningExtensions {

    public static IServiceCollection AddApiVersion(this IServiceCollection services) {

        // API Versioning
        services
            .AddApiVersioning(options => {
                options.DefaultApiVersion = new ApiVersion(1, 0);
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.ReportApiVersions = true;
                options.ApiVersionReader = ApiVersionReader.Combine(
                    new UrlSegmentApiVersionReader(),
                    new QueryStringApiVersionReader("api-version"),
                    new HeaderApiVersionReader("X-API-Version"));
            })
            .AddApiExplorer(options => {
                options.GroupNameFormat = "'v'VVV";
                options.SubstituteApiVersionInUrl = true;
            });
        
        return services;
    }

  
}
