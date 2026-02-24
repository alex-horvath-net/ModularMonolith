using Core.Infrastructure;
using Features.Accounts.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace Features.Accounts.Slices.Register;

public static class Extensions {
    public static IServiceCollection AddRegistration(this IServiceCollection services) {
        services.AddScoped<UserStory>(sp => new(
            sp.GetRequiredService<IAccountRepository>(),
            sp.GetRequiredService<IHasher>(),
            sp.GetRequiredService<IClock>()));

        services.AddScoped<Blazor.IGateway>(sp => new Blazor.Gateway(
            sp.GetRequiredService<UserStory>()));

        return services;
    }
}
