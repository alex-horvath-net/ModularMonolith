using System;
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
        services.AddDbContext<OrdersDbContext>((sp, options) => {
            var env = sp.GetRequiredService<IHostEnvironment>();
            var loggerFactory = sp.GetRequiredService<ILoggerFactory>();
            options.UseLoggerFactory(loggerFactory);
            options.EnableDetailedErrors(env.IsDevelopment());
            options.EnableSensitiveDataLogging(env.IsDevelopment());
            options.UseSqlServer(
                configuration.GetConnectionString("OrdersDB"),
                sql => {
                    sql.EnableRetryOnFailure(5, TimeSpan.FromSeconds(2), null);
                    sql.CommandTimeout(30);
                });
        });

        return services;
    }

    // Maps Orders endpoints and performs dev-time DB initialization
    public static IEndpointRouteBuilder MapOrders(this WebApplication app) {
        app.CreateAndMigrateDB();
        app.MapOrdersEndpoints();
        return app;
    }

    private static IEndpointRouteBuilder CreateAndMigrateDB(this WebApplication app) {
        using var scope = app.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<OrdersDbContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("OrdersDbInit");

        if (app.Environment.IsDevelopment()) {
            try {
                db.Database.EnsureCreated();
                db.Database.Migrate();
            } catch (Exception ex) {
                logger.LogError(ex, "Failed to initialize Orders database.");
            }
        }
        return app;
    }
}