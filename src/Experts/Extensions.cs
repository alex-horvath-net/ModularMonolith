using Business.Events;
using Microsoft.Extensions.DependencyInjection;

namespace Business;

public static class Extensions {
    public static IServiceCollection AddShared(this IServiceCollection services) {
        // Business Events - in-process publisher
        services.AddScoped<IBusinessEventPublisher, InProcessBusinessEventPublisher>();
        return services;
    }
}
