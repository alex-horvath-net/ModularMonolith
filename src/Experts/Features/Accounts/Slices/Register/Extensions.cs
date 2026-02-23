using Microsoft.Extensions.DependencyInjection;

namespace Core.Features.Accounts.Slices.Register;

public static class Extensions {
    public static IServiceCollection AddRegistration(this IServiceCollection services) {
        services.AddScoped<UserStory>();
        services.AddScoped<BlazorGateway>();
        return services;
    }
}
