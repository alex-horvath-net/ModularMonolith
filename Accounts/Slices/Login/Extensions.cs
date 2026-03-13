using Accounts.Infrastructure;
using Accounts.Slices.Login.Blazor;
using Core.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace Accounts.Slices.Login;

public static class Extensions {
    public static IServiceCollection AddLogion(this IServiceCollection services) {
        services.AddScoped<UserStory>(sp => new(
            sp.GetRequiredService<IAccountRepository>(),
            sp.GetRequiredService<IHasher>()));

        services.AddScoped<ILogin>(sp => new LoginUserStoryAdapter(
            sp.GetRequiredService<UserStory>()));

        return services;
    }
}
