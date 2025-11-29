using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Common.Publish;
internal static class ApiAdminExtensions {

    internal static IServiceCollection AddApiAdmin(this IServiceCollection services) {

        // Registers OpenAPI/Swagger generation (dev only exposure later)
        services.AddSwaggerGen(options => {
        });

        services.ConfigureSwagger(options => {

        });

        services.ConfigureSwaggerGen(options => {

        });



        return services;
    }
    internal static IEndpointRouteBuilder MapApiAdmin(this WebApplication app) {

        if (app.Environment.IsDevelopment()) {

            // Serve Swagger UI at /swagger and point it to the OpenAPI document
            app.UseSwaggerUI(c => {
                c.SwaggerEndpoint("/openapi/document.json", "WebApi");
                c.RoutePrefix = "swagger"; // UI at /swagger
            });

        }

        return app; 
    }
}
