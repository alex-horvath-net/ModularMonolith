using Asp.Versioning;
using Experts.OrderExpert.PlaceOrderFlow.Shared.Business.Domain;
using Experts.OrderExpert.Shared.Business;
using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Experts.OrderExpert.PlaceOrderFlow.Shared.Infrastructure;

public static class CreateOrderEndpoint
{
    public static IEndpointRouteBuilder MapCreateOrderEndpoint(this IEndpointRouteBuilder app)
    {
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
        PlaceOrderWorkflow workflow,
        IValidator<CreateOrderRequest> validator,
        CreateOrderRequest request,
        CancellationToken token)
    {
        var validation = await validator.ValidateAsync(request, token);
        if (!validation.IsValid)
        {
            return TypedResults.BadRequest(new
            {
                errors = validation.Errors.Select(e => new { e.PropertyName, e.ErrorMessage })
            });
        }

        var response = await workflow.Run(request, token);
        return TypedResults.Created($"/v1/orders/{response.Order.Id}", response.Order.Id);
    }
}
 