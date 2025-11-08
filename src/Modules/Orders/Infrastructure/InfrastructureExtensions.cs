using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;

namespace Orders.Infrastructure;

public static class InfrastructureExtensions {
    public static IServiceCollection AddOrdersInfrastructure(this IServiceCollection services, IConfiguration configuration) {
        services.AddDbContext<OrdersDbContext>(opts =>
        opts.UseSqlServer(configuration.GetConnectionString("Default")));
        return services;
    }
}
