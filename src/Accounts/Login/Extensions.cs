using Accounts.Core.Infrastructure;
using Accounts.Login.Blazor;
using Core.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace Accounts.Login;

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
