using Asp.Versioning;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Hosting;
using Orders.Featrures.Create;

namespace Common.Authentication;

public static class CreateTokenEndPoint {

    public static IEndpointRouteBuilder MapDevToken(this WebApplication app) {

        if (app.Environment.IsDevelopment()) {

            var version = app.NewApiVersionSet()
                .HasApiVersion(new ApiVersion(1, 0))
                .ReportApiVersions()
                .Build();

            var group = app.MapGroup("/v{version:apiVersion}/devtokens")
                .WithApiVersionSet(version)
                .WithTags("DevTokens");

            group.MapPost("/", CreateDevToken)
                .AllowAnonymous()
                .RequireRateLimiting("writes")
                .Produces(StatusCodes.Status201Created)
                .ProducesValidationProblem();
        }

        return app;
    }

    private static async Task<IResult> CreateDevToken(
        CreateTokenCommandHandler handler,
        CreateTokenCommand command) {
       
        var result = new {
            access_token = await handler.Handle(command),
            token_type = "Bearer",
            expires_in = 600
        };

        return Results.Ok(result);
    }
}
