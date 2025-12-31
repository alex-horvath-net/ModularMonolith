using Asp.Versioning;
using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Orders.Create.CommandHandlers;

namespace BusinessExperts.Orders.Featrures.Create;

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
            .RequireAuthorization(OrdersConstants.Write)
            .RequireRateLimiting("writes")
            .Produces(StatusCodes.Status201Created)
            .ProducesValidationProblem();

        return app;
    }

    private static async Task<IResult> Handle(
        CreateOrderCommandHandler commandHandler,
        IValidator<CreateOrderCommand> validator,
        CreateOrderCommand command,
        CancellationToken token)
    {
        var validation = await validator.ValidateAsync(command, token);
        if (!validation.IsValid)
        {
            return TypedResults.BadRequest(new
            {
                errors = validation.Errors.Select(e => new { e.PropertyName, e.ErrorMessage })
            });
        }

        var id = await commandHandler.Handle(command, token);
        return TypedResults.Created($"/v1/orders/{id}", new { id });
    }
}
