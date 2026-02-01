using Experts.SecurityOfficer.Common.Security;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Experts.SecurityOfficer.Login;

public static class Extensions {
    public static IServiceCollection AddLogion(this IServiceCollection services) {
        services.AddScoped<UserStoryBlazorClient>();
        services.AddScoped<UserStory>();
        services.AddScoped<Authenticate>();
        services.AddScoped<Authenticate.IStore, Authenticate.Store>();
        services.AddScoped<Authorize>();
        services.AddScoped<Authorize.IStore, Authorize.Store>();
        services.TryAddSingleton<IPasswordHasher, Pbkdf2PasswordHasher>();
        return services;
    }
}
