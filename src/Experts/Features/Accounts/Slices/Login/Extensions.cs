using Microsoft.Extensions.DependencyInjection;

namespace Business.Features.Accounts.Slices.Login;

public static class Extensions {
    public static IServiceCollection AddLogion(this IServiceCollection services) {
        services.AddScoped<UserStoryBlazorClient>();
        services.AddScoped<UserStory>();
        return services;
    }
}
