using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Orders.API;
using Orders.Application;
using Orders.Infrastructure;

namespace Orders;

public static class Extensions {
    // Registers infrastructure and application services for the Orders module
    public static IServiceCollection AddOrders(this IServiceCollection services, IConfiguration configuration) {

        // Application
        services.AddScoped<GetOrdersQueryHandler>();
        services.AddScoped<GetOrderQueryHandler>();
        services.AddScoped<CreateOrderCommandHandler>();
         
        // Infrastructure
        services.AddDbContext<OrdersDbContext>(options => options
            .EnableDetailedErrors()
            .EnableSensitiveDataLogging()
            .UseLoggerFactory(LoggerFactory.Create(builder => builder.AddConfiguration(configuration)))
            .UseSqlServer(configuration.GetConnectionString("OrdersDB"), sqlOptions => sqlOptions.EnableRetryOnFailure(5, TimeSpan.FromSeconds(2), null)));


        return services;
    }

    // Maps Orders endpoints and performs dev-time DB initialization
    public static IEndpointRouteBuilder MapOrders(this WebApplication app) {
        app.CreateDB();
        app.MapOrdersEndpoints();
        return app;
    }

    private static IEndpointRouteBuilder CreateDB(this WebApplication app) {
        using var scope = app.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<OrdersDbContext>();
        if (app.Environment.IsDevelopment()) {
            db.Database.EnsureCreated();
        }

        return app;
    }
}
