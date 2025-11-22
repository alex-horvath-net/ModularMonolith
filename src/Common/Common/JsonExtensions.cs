using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;

namespace Common; 
internal static class JsonExtensions {



    public static IServiceCollection AddJson(this IServiceCollection services) {
        services.ConfigureHttpJsonOptions(o => {
            o.SerializerOptions.ReadCommentHandling = JsonCommentHandling.Disallow;
            o.SerializerOptions.MaxDepth = 32;
            o.SerializerOptions.PropertyNameCaseInsensitive = false;
        });
        return services;
    }
}