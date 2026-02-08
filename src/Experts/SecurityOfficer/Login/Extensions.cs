using Experts.SecurityOfficer.Common.Infrastructure.Security;
using Microsoft.Extensions.DependencyInjection;

namespace Experts.SecurityOfficer.Login;

public static class Extensions {
    public static IServiceCollection AddLogion(this IServiceCollection services) {
        services.AddScoped<UserStoryBlazorClient>();
        services.AddScoped<UserStory>();
        services.AddScoped<IAuthenticateStore, AuthenticateStore>();
        services.AddScoped<IRandomNumberGenerator, RandomNumberGenerator>();
        return services;
    }
}
