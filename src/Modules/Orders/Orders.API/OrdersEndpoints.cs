using Orders.Application;

namespace Orders.API;

public static class OrdersEndpoints {
    public static IEndpointRouteBuilder MapOrders(this IEndpointRouteBuilder app) {
        var group = app.MapGroup("/orders");

        // POST /orders
        group.MapPost("/", async (CreateOrderRequest req, CreateOrderHandler handler) => {
            var id = await handler.Handle(req);
            return TypedResults.Created($"/orders/{id}", new { id });
        });

        // GET /orders/{id}
        group.MapGet("/{id:guid}", async Task<IResult> (Guid id, GetOrderHandler handler) => {
            var dto = await handler.Handle(id);
            return dto is null ? TypedResults.NotFound() : TypedResults.Ok(dto);
        });

        return app;
    }
}
