using Asp.Versioning;
using Business.Modules.OrderExpert.Common.Business;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Business.Modules.OrderExpert.PlaceOrder;

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
        PlaceOrderRequest request,
        CancellationToken token) {

        var response = await placeOrder.Run(request, token);
        if (response.Errors.Any())             return TypedResults.BadRequest(response.Errors);

        if (response.Order is null)             return TypedResults.Problem("Order was not created.", statusCode: StatusCodes.Status500InternalServerError);

        return TypedResults.Created($"/v1/orders/{response.Order.Id}", response.Order.Id);
    }
}
