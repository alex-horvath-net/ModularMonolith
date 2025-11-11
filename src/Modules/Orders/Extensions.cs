using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Orders.Infrastructure;

namespace Orders;

public static class OrdersEndpointMapping
{
    //public static IEndpointRouteBuilder MapOrders(this IEndpointRouteBuilder app) {
    public static IEndpointRouteBuilder MapOrders(this WebApplication app) {

        // Ensure databases exist (development-friendly). For production, prefer migrations.
        using (var scope = app.Services.CreateScope()) {
            var sp = scope.ServiceProvider;
           
            var ordersDb = sp.GetRequiredService<OrdersDbContext>();
            ordersDb.Database.EnsureCreated();

        }

        return API.OrdersEndpoints.MapOrders(app);
    }
}
