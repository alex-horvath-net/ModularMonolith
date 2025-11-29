using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

namespace Common.Publish; 

internal static class ApiDocumentationExtensions {

    internal static IServiceCollection AddApiDocumentation(this IServiceCollection services) {

        services.AddOpenApi("document", options => {

            // Add JWT Bearer security scheme so Swagger UI shows the Authorize button
            options.AddDocumentTransformer((document, ctx, ct) => {
                document.Components ??= new();
                document.Components.SecuritySchemes["Bearer"] = new OpenApiSecurityScheme {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Input: Bearer {token}"
                };
                return Task.CompletedTask;
            });

            options.AddOperationTransformer((operation, ctx, ct) => {
                // Apply Bearer auth requirement to all operations (Swagger UI will send JWT when authorized)
                operation.Security ??= new List<OpenApiSecurityRequirement>();
                operation.Security.Add(new OpenApiSecurityRequirement {
                    [
                        new OpenApiSecurityScheme {
                            Reference = new OpenApiReference {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        }
                    ] = Array.Empty<string>()
                });
                return Task.CompletedTask;
            });
        });

        return services;
    }

    internal static IEndpointRouteBuilder MapApiDocumentation(this WebApplication app) {

        if (app.Environment.IsDevelopment()) {
            // /openapi/{documentName}.json
            var openApi = app.MapOpenApi(); // Expose OpenAPI only in dev
            openApi.AllowAnonymous();       // Make OpenAPI spec accessible without auth 
            openApi.DisableRateLimiting();  // Do not rate-limit Swagger/OpenAPI
        }

        return app;
    }
}