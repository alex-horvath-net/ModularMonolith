using Asp.Versioning;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Hosting;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace BusinessExperts.Identity.CreateToken;

public static class CreateTokenEndPoint {

    public static IEndpointRouteBuilder MapDevToken(this WebApplication app) {

        if (app.Environment.IsDevelopment() || string.Equals(app.Environment.EnvironmentName, "IntegrationTest", StringComparison.OrdinalIgnoreCase)) {

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

        var response = new CreateTokenResponse(
            access_token: await handler.Handle(command),
            token_type: "Bearer",
            expires_in: 600
        );

        return Results.Created(string.Empty, response);
    }
}

public record CreateTokenResponse(
    string access_token,
    string token_type,
    int expires_in);
