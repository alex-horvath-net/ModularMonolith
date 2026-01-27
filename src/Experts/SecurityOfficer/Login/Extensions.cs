using Microsoft.Extensions.DependencyInjection;

namespace Experts.SecurityOfficer.Login; 
public static class Extensions {
    public static IServiceCollection AddLogion(this IServiceCollection services) {
        services.AddScoped<UserStoryBlazorClient>();
        services.AddScoped<UserStory>();
        services.AddScoped<Authenticate>();
        services.AddScoped<Authenticate.IStore, Authenticate.Store>();
        services.AddScoped<Authenticate.IHasher, Authenticate.BCryptHasher>();
        return services;
    }
}
