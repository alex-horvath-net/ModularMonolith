using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Orders.Infrastructure;
public static class InfrastructureExtensions {
    public static IServiceCollection AddOrdersInfrastructure(this IServiceCollection services, IConfiguration configuration) => services
        .AddDbContext<OrdersDbContext>();
}
