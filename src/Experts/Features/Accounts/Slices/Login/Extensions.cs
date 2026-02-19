using Business.Features.Accounts.Infrastructure;
using Business.Features.Accounts.Infrastructure.Random;
using Microsoft.Extensions.DependencyInjection;

namespace Business.Features.Accounts.Slices.Login;

public static class Extensions {
    public static IServiceCollection AddLogion(this IServiceCollection services) {
        services.AddScoped<UserStoryBlazorClient>();
        services.AddScoped<UserStory>();
        services.AddScoped<IAuthenticateStore, AuthenticateStore>();
        services.AddScoped<IRandom, RandomGenerator>();
        return services;
    }
}
