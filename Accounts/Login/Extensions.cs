using Accounts.Core.Infrastructure;
using Accounts.Features.Login.Blazor;
using Accounts.Login.Blazor;
using Accounts.Slices.Login.Blazor;
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
