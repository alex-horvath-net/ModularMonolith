using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Orders.Application.CommandHandlers;
using Orders.Application.QueryHandlers;
using Orders.Contracts.DTOs;

namespace Orders.API;

//  Microsoft.AspNetCore.Routing
//  Microsoft.AspNetCore.Builder
public static class OrdersEndpoints {
    /// <summary>
    /// It encapsulates internals, so host doesn't have to know them.
    /// </summary>
    /// <param name="app"></param>
    /// <returns></returns>
    public static IEndpointRouteBuilder MapOrders(this IEndpointRouteBuilder app) {
        RouteGroupBuilder group = app.MapGroup("/orders");

        // POST /orders
        group.MapPost("/", async (CreateOrderCommand req, CreateOrderCommandHandler handler) => {
            Guid id = await handler.Handle(req);
            return TypedResults.Created($"/order/{id}", new { id });
        });

        // GET /orders/{id}
        group.MapGet("/{id:guid}", async Task<IResult> (Guid id, GetOrderQueryHandler handler) => {
            OrderDto dto = await handler.Handle(id);
            return dto is null ? TypedResults.NotFound() : TypedResults.Ok(dto);
        });

        return app;
    }
}
