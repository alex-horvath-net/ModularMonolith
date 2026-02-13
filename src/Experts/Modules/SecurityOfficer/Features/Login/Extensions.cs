using Business.Modules.SecurityOfficer.Infrastructure.Random;
using Business.Modules.SecurityOfficer.Infrastructure.Cryptography;
using Microsoft.Extensions.DependencyInjection;

namespace Business.Modules.SecurityOfficer.Features.Login;

public static class Extensions {
    public static IServiceCollection AddLogion(this IServiceCollection services) {
        services.AddScoped<UserStoryBlazorClient>();
        services.AddScoped<UserStory>();
        services.AddScoped<IAuthenticateStore, AuthenticateStore>();
        services.AddScoped<IRandom, RandomGenerator>();
        return services;
    }
}
