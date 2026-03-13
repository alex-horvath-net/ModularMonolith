using Accounts.Infrastructure;
using Accounts.Slices.Register.Triggers.Blazor;
using Core.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace Accounts.Slices.Register;

public static class Extensions {
    public static IServiceCollection AddRegistration(this IServiceCollection services) {
        services.AddScoped<UserStory.UserStory>(sp => new(
            sp.GetRequiredService<IAccountRepository>(),
            sp.GetRequiredService<IHasher>(),
            sp.GetRequiredService<IClock>()));

        services.AddScoped<IRegister>(sp => new UserStoryAdapter(
            sp.GetRequiredService<UserStory.UserStory>()));

        return services;
    }
}
