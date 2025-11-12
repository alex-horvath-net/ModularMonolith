using Microsoft.AspNetCore.Builder; // MapGroup extension
using Microsoft.AspNetCore.Http; // IResult, TypedResults
using Microsoft.AspNetCore.Routing;
using Orders.Application.CommandHandlers;
using Orders.Application.QueryHandlers;
using Orders.Contracts.DTOs; // For swagger metadata

namespace Orders.API;

public static class OrdersEndpoints {
    public static IEndpointRouteBuilder MapOrdersEndpoints(this IEndpointRouteBuilder app) {
        var group = app.MapGroup("/orders").WithTags("Orders")
            //.RequireAuthorization("Orders.ReadWrite")
            ; // define policy at startup

        // GET /orders (list)
        group.MapGet("", async (GetOrdersQueryHandler handler, CancellationToken token) => {
            var orders = await handler.Handle(token);
            return TypedResults.Ok(orders);
        })
        .Produces<List<OrderDto>>(StatusCodes.Status200OK);

        // GET /orders/{id}
        group.MapGet("/{id:guid}", async Task<IResult> (Guid id, GetOrderQueryHandler handler) => {
            var dto = await handler.Handle(id);
            return dto is null ? TypedResults.NotFound() : TypedResults.Ok(dto);
        })
        .Produces<OrderDto>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound);

        // POST /orders
        group.MapPost("/", async (CreateOrderCommand req, CreateOrderCommandHandler handler) => {
            var id = await handler.Handle(req);
            return TypedResults.Created($"/orders/{id}", new { id });
        })
        .Produces(StatusCodes.Status201Created);

        return app;
    }
}
