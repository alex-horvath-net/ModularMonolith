using Core.Infrastructure;
using Features.Accounts.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace Features.Accounts.Slices.Login;

public static class Extensions {
    public static IServiceCollection AddLogion(this IServiceCollection services) {
        services.AddScoped<UserStory>(sp => new(
            sp.GetRequiredService<IAccountRepository>(),
            sp.GetRequiredService<IHasher>()));

        services.AddScoped<IBlazorGateway>(sp => new BlazorGateway(
            sp.GetRequiredService<UserStory>()));

        return services;
    }
}
