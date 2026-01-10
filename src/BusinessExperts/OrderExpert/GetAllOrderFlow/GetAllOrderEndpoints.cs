using Asp.Versioning;
using Experts.OrderExpert.GetOrderFlow;
using Experts.OrderExpert.Shared.Business;
using Microsoft.AspNetCore.Builder; // MapGroup extension
using Microsoft.AspNetCore.Http; // IResult, TypedResults
using Microsoft.AspNetCore.Http.HttpResults; // OK
using Microsoft.AspNetCore.Routing;

namespace Experts.OrderExpert.GetAllOrderFlow;

public static class GetAllOrderEndpoints {
    public static IEndpointRouteBuilder MapOrdersEndpoints(this IEndpointRouteBuilder app) {
        var versionSet = app.NewApiVersionSet().HasApiVersion(new ApiVersion(1, 0)).ReportApiVersions().Build();
        var group = app.MapGroup("/v{version:apiVersion}/orders")
            .WithApiVersionSet(versionSet)
            .WithTags("Orders");

        group.MapGet("", GetAllOrders)
            .RequireAuthorization(OrdersConstants.Read)
            .Produces<List<Shared.Business.Domain.Order>>(StatusCodes.Status200OK);

        group.MapGet("/{id:guid}", GetOrderById)
            .RequireAuthorization(OrdersConstants.Read)
            .Produces<Shared.Business.Domain.Order>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);

        return app;
    }

    private static async Task<Ok<List<Shared.Business.Domain.Order>>> GetAllOrders(
        GetAllOrderQueryHandler queryHandler,
        CancellationToken token) {
        var orders = await queryHandler.Handle(token);
        return TypedResults.Ok(orders);
    }

    private static async Task<IResult> GetOrderById(
        GetOrderQueryHandler queryHandler,
        Guid id,
        CancellationToken token) {
        var order = await queryHandler.Handle(id, token);
        return order is null ? TypedResults.NotFound() : TypedResults.Ok(order);
    }
}
