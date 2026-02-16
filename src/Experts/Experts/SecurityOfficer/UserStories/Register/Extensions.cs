using Business.Experts.SecurityOfficer.Infrastructure.Clock;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Business.Experts.SecurityOfficer.UserStories.Register;

public static class Extensions {
    public static IServiceCollection AddRegistration(this IServiceCollection services) {
        services.AddScoped<UserStory>();
        services.AddScoped<UserStoryBlazorClient>();
        services.AddScoped<UserStory.IAccountStore, AccountStore>();
        services.TryAddSingleton<UserStory.IClock, SystemClock>();
        return services;
    }
}
