using Microsoft.AspNetCore.Routing;

namespace Orders;

public static class OrdersEndpointMapping
{
 public static IEndpointRouteBuilder MapOrders(this IEndpointRouteBuilder app)
 => API.OrdersEndpoints.MapOrders(app);
}
