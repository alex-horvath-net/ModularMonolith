using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Orders.Application;
using Orders.Infrastructure;
using Microsoft.AspNetCore.Routing;

namespace Orders.Extensions;

public static class Extensions {
    public static void AddOrders(this IServiceCollection services, IConfiguration configuration) => services
        .AddOrdersInfrastructure(configuration)
        .AddOrdersApplication();

    // Expose Orders API endpoint mapping via Extensions to avoid direct reference from hosts
    public static IEndpointRouteBuilder MapOrders(this IEndpointRouteBuilder app)
        => Orders.API.OrdersEndpoints.MapOrders(app);
}
