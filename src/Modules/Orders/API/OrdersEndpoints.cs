using FluentValidation;
using Microsoft.AspNetCore.Builder; // MapGroup extension
using Microsoft.AspNetCore.Http; // IResult, TypedResults
using Microsoft.AspNetCore.Http.HttpResults; // OK
using Microsoft.AspNetCore.Routing; // IEndpointRouteBuilder
using Orders.Application.CommandHandlers;
using Orders.Application.QueryHandlers;
using Orders.Contracts.DTOs; // For swagger metadata
using Asp.Versioning; // API versioning

namespace Orders.API;
public static class OrdersEndpoints {
    public static IEndpointRouteBuilder MapOrdersEndpoints(this IEndpointRouteBuilder app) {
        var versionSet = app.NewApiVersionSet().HasApiVersion(new ApiVersion(1,0)).ReportApiVersions().Build();
        var group = app.MapGroup("/v{version:apiVersion}/orders")
            .WithApiVersionSet(versionSet)
            .WithTags("Orders");

        group.MapGet("", GetOrders)
            .RequireAuthorization(OrdersConstants.Read)
            .Produces<List<OrderDto>>(StatusCodes.Status200OK);

        group.MapGet("/{id:guid}", GetOrder)
            .RequireAuthorization(OrdersConstants.Read)
            .Produces<OrderDto>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);

        group.MapPost("/", CreateOrder)
            .RequireAuthorization(OrdersConstants.Write)
            .RequireRateLimiting("writes")
            .Produces(StatusCodes.Status201Created);

        return app;
    }

    private static async Task<Ok<List<OrderDto>>> GetOrders(GetOrdersQueryHandler handler, CancellationToken token) {
        var orders = await handler.Handle(token);
        return TypedResults.Ok(orders);
    }

    private static async Task<IResult> GetOrder(GetOrderQueryHandler handler, Guid id, CancellationToken token) {
        var order = await handler.Handle(id, token);
        return order is null ? TypedResults.NotFound() : TypedResults.Ok(order);
    }

    private static async Task<IResult> CreateOrder(CreateOrderCommandHandler handler, IValidator<CreateOrderCommand> validator, CreateOrderCommand command, CancellationToken token) {
        var validation = await validator.ValidateAsync(command, token);
        if (!validation.IsValid) {
            return TypedResults.BadRequest(new { errors = validation.Errors.Select(e => new { e.PropertyName, e.ErrorMessage }) });
        }
        var id = await handler.Handle(command, token);
        return TypedResults.Created($"/v1/orders/{id}", new { id });
    }
}
