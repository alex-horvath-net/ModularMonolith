using Core.Events;
using Core.Infrastructure;
using Core.Infrastructure.Clock;
using Core.Infrastructure.GuidNumber;
using Core.Infrastructure.Hash;
using Core.Infrastructure.Random;
using Microsoft.Extensions.DependencyInjection;

namespace Core;

public static class Extensions {
    public static IServiceCollection AddCore(this IServiceCollection services) {
        services.AddScoped<IBusinessEventPublisher, InProcessBusinessEventPublisher>();
        services.AddSingleton<IRandom, RandomGenerator>();
        services.AddSingleton<IHasher, Pbkdf2HashGenerator>();
        services.AddSingleton<IGuid, GuidGenerator>();
        services.AddSingleton<IClock, SystemClock>();
        return services;
    }
}
