using Microsoft.AspNetCore.Builder; // MapGroup extension
using Microsoft.AspNetCore.Http; // IResult, TypedResults
using Microsoft.AspNetCore.Routing;
using Orders.Application; // Handlers & commands

namespace Orders.API;

public static class OrdersEndpoints {
    public static IEndpointRouteBuilder MapOrders(this IEndpointRouteBuilder app) {
        var group = app.MapGroup("/orders");

        group.MapPost("/", async (CreateOrderCommand req, CreateOrderCommandHandler handler) => {
            var id = await handler.Handle(req);
            return TypedResults.Created($"/orders/{id}", new { id });
        });

        group.MapGet("/{id:guid}", async Task<IResult> (Guid id, GetOrderQueryHandler handler) => {
            var dto = await handler.Handle(id);
            return dto is null ? TypedResults.NotFound() : TypedResults.Ok(dto);
        });

        return app;
    }
}
