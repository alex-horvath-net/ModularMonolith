using Business.Modules.SecurityOfficer.Features.Register.Infrastructure;
using Business.Modules.SecurityOfficer.Infrastructure.Clock;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Business.Modules.SecurityOfficer.Features.Register;

public static class Extensions {
    public static IServiceCollection AddRegistration(this IServiceCollection services) {
        services.AddScoped<UserStory>();
        services.AddScoped<UserStoryBlazorClient>();
        services.AddScoped<UserStory.IAccountStore, AccountStore>();
        services.TryAddSingleton<UserStory.IClock, SystemClock>();
        return services;
    }
}
