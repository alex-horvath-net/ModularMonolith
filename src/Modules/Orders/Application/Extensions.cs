using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Orders.Application;
using Orders.Infrastructure;

namespace Orders;

public static class OrdersModuleExtensions {
    public static IServiceCollection AddOrders(this IServiceCollection services, IConfiguration configuration) {
        services
        .AddOrdersInfrastructure(configuration)
        .AddOrdersApplication();
        return services;
    }
}
