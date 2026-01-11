using Asp.Versioning;
using Experts.OrderExpert.PlaceOrderFlow.Shared.Business.Domain;
using Experts.OrderExpert.Shared.Business;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Experts.OrderExpert.PlaceOrderFlow;

public static class ApiEndpoint {
    public static IEndpointRouteBuilder MapCreateOrderEndpoint(this IEndpointRouteBuilder app) {

        var versionSet = app.NewApiVersionSet().HasApiVersion(new ApiVersion(1, 0)).ReportApiVersions().Build();

        var routeGroup = app.MapGroup("/v{version:apiVersion}/orders")
            .WithApiVersionSet(versionSet)
            .WithTags("OrderExpert");

        routeGroup.MapPost("/", Handler)
            .RequireRateLimiting("writes")
            .RequireAuthorization(OrdersConstants.Write)
            .Produces(StatusCodes.Status201Created)
            .ProducesValidationProblem()
            .WithTags("PlaceOrderFlow");

        return app;
    }

    private static async Task<IResult> Handler(
        BusinessWorkFlow placeOrder,
        CreateOrderRequest request,
        CancellationToken token) {

        var response = await placeOrder.Run(request, token);
        return
            response.Errors.Any() ?
            TypedResults.BadRequest(response.Errors) :
            TypedResults.Created($"/v1/orders/{response.Order.Id}", response.Order.Id);
    }
}
