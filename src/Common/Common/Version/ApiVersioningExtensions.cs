using Asp.Versioning;
using Microsoft.Extensions.DependencyInjection;

namespace Common.Version;
public static class ApiVersioningExtensions {

    public static IServiceCollection AddApiVersion(this IServiceCollection services) {

        // API Versioning
        services.AddApiVersioning(o => {
            o.DefaultApiVersion = new ApiVersion(1, 0);
            o.AssumeDefaultVersionWhenUnspecified = true;
            o.ReportApiVersions = true;
            o.ApiVersionReader = ApiVersionReader.Combine(
                new UrlSegmentApiVersionReader(), 
                new HeaderApiVersionReader("X-API-Version"));
        }).AddApiExplorer(o => {
            o.GroupNameFormat = "'v'VVV";
            o.SubstituteApiVersionInUrl = true;
        });
        
        return services;
    }

  
}
