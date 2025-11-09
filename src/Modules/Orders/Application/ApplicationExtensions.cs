using Microsoft.Extensions.DependencyInjection;
using Orders.Contracts.Services;

namespace Orders.Application;

public static class ApplicationExtensions {
    public static IServiceCollection AddOrdersApplication(this IServiceCollection services) {
        services.AddTransient<CreateOrderCommandHandler>();
        services.AddTransient<GetOrderQueryHandler>();
        services.AddTransient<GetOrdersQueryHandler>(); // Added missing registration
        services.AddTransient<IReadOrderService, ReadOrderService>();
        return services;
    }
}
