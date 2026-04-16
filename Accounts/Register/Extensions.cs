using Accounts.Core.Infrastructure;
using Accounts.Register.Triggers.Blazor;
using Core.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace Accounts.Register;

internal static class Extensions {
    internal static IServiceCollection AddRegistration(this IServiceCollection services) {
        services.AddScoped<UserStory.UserStory>(sp => new(
            sp.GetRequiredService<IAccountRepository>(),
            sp.GetRequiredService<IHasher>(),
            sp.GetRequiredService<IClock>(),
            sp.GetRequiredService<IGuidGenerator>(),
            sp.GetRequiredService<ILogger<UserStory.UserStory>>()));

        services.AddScoped<IRegister>(sp => new UserStoryAdapterForBlazor(
            sp.GetRequiredService<UserStory.UserStory>()));

        return services;
    }
}
