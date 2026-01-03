using Asp.Versioning;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Hosting;

namespace BusinessExperts.Identity.CreateToken;

public static class CreateTokenEndPoint {

    public static IEndpointRouteBuilder MapDevToken(this WebApplication app) {

        if (app.Environment.IsDevelopment() || app.Environment.IsEnvironment("IntegrationTest")) {

            var version = app.NewApiVersionSet()
                .HasApiVersion(new ApiVersion(1, 0))
                .ReportApiVersions()
                .Build();

            var group = app.MapGroup("/v{version:apiVersion}/devtokens")
                .WithApiVersionSet(version)
                .WithTags("DevTokens");

            group.MapPost("/", CreateDevTokenHandler)
                .AllowAnonymous()
                .RequireRateLimiting("writes")
                .Produces(StatusCodes.Status201Created)
                .ProducesValidationProblem();
        }

        return app;
    }

    private static async Task<IResult> CreateDevTokenHandler(CreateTokenCommandHandler handler, CreateTokenCommand command) {

        var accessToken = await handler.Handle(command);

        return Results.Created(string.Empty, accessToken);
    }
}