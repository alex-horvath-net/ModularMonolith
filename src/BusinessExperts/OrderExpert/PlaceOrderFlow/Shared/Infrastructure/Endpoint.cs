using Asp.Versioning;
using Experts.OrderExpert.PlaceOrderFlow.Shared.Business.Domain;
using Experts.OrderExpert.Shared.Business;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Experts.OrderExpert.PlaceOrderFlow.Shared.Infrastructure;

public static class Endpoint {
    public static IEndpointRouteBuilder MapCreateOrderEndpoint(this IEndpointRouteBuilder app) {
        var versionSet = app.NewApiVersionSet()
            .HasApiVersion(new ApiVersion(1, 0))
            .ReportApiVersions()
            .Build();

        var group = app.MapGroup("/v{version:apiVersion}/orders")
            .WithApiVersionSet(versionSet)
            .WithTags("Orders");

        group.MapPost("/", Handle)
            .RequireRateLimiting("writes")
            .RequireAuthorization(OrdersConstants.Write)
            .Produces(StatusCodes.Status201Created)
            .ProducesValidationProblem();

        return app;
    }

    private static async Task<IResult> Handle(
        OrderBusinessExpert order,
        CreateOrderRequest request,
        CancellationToken token) {

        var response = await order.PlaceOrder.Run(request, token);
        return
            response.Errors.Any() ?
            TypedResults.BadRequest(response.Errors) :
            TypedResults.Created($"/v1/orders/{response.Order.Id}", response.Order.Id);
    }
}
