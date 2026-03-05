using Core.Infrastructure;
using Features.Accounts.Infrastructure;
using Features.Accounts.Slices.Register.Triggers.Blazor;
using Microsoft.Extensions.DependencyInjection;

namespace Features.Accounts.Slices.Register;

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
