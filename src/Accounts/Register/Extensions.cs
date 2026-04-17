using Accounts.Core.Infrastructure;
using Accounts.Register.BlazorTrigger;
using Core.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace Accounts.Register;

internal static class Extensions {
    internal static IServiceCollection AddRegistration(this IServiceCollection services) {
        services.AddScoped<UserStory>(sp => new(
            sp.GetRequiredService<IAccountRepository>(),
            sp.GetRequiredService<IHasher>(),
            sp.GetRequiredService<IClock>(),
            sp.GetRequiredService<IGuidGenerator>(),
            sp.GetRequiredService<ILogger<UserStory>>()));

        services.AddScoped<IRegisterAdapter>(sp => new RegisterUserStoryAdapter(
            sp.GetRequiredService<UserStory>()));

        return services;
    }
}
