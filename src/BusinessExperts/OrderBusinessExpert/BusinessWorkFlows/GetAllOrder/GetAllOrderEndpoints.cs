using Asp.Versioning;
using BusinessExperts.OrderBusinessExpert.BusinessWorkFlows.GetOrderById;
using BusinessExperts.OrderBusinessExpert.BusinessWorkFlows.Shared.Business;
using BusinessExperts.OrderBusinessExpert.BusinessWorkFlows.Shared.Business.Domain;
using Microsoft.AspNetCore.Builder; // MapGroup extension
using Microsoft.AspNetCore.Http; // IResult, TypedResults
using Microsoft.AspNetCore.Http.HttpResults; // OK
using Microsoft.AspNetCore.Routing;

namespace BusinessExperts.OrderBusinessExpert.BusinessWorkFlows.GetAllOrder;
public static class GetAllOrderEndpoints {
    public static IEndpointRouteBuilder MapOrdersEndpoints(this IEndpointRouteBuilder app) {
        var versionSet = app.NewApiVersionSet().HasApiVersion(new ApiVersion(1,0)).ReportApiVersions().Build();
        var group = app.MapGroup("/v{version:apiVersion}/orders")
            .WithApiVersionSet(versionSet)
            .WithTags("Orders");

        group.MapGet("", GetAllOrders)
            .RequireAuthorization(OrdersConstants.Read)
            .Produces<List<Order>>(StatusCodes.Status200OK);

        group.MapGet("/{id:guid}", GetOrderById)
            .RequireAuthorization(OrdersConstants.Read)
            .Produces<Order>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);

        return app;
    }

    private static async Task<Ok<List<Order>>> GetAllOrders(
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
