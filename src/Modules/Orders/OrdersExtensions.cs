using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Orders.Create.API;
using Orders.Create.Application.CommandHandlers;
using Orders.Create.Application.QueryHandlers;
using Orders.Create.Application.QueryServices;
using Orders.Create.Application.Validation;
using Orders.Create.Contracts.Services;
using Orders.Create.Infrastructure.Data;

namespace Orders;

public static class OrdersExtensions {
    public static IServiceCollection AddOrders(this IServiceCollection services, IConfiguration configuration) {

        // Application
        services.AddScoped<GetOrdersQueryHandler>();
        services.AddScoped<GetOrderQueryHandler>();
        services.AddScoped<CreateOrderCommandHandler>();
        services.AddScoped<IReadOrderService, ReadOrderService>();

        // Register validator so Minimal APIs resolve it from DI (not Body)
        services.AddScoped<IValidator<CreateOrderCommand>, CreateOrderCommandValidator>();

        // Infrastructure
        services.AddDbContext<OrdersDbContext>((sp, options) => {
            var env = sp.GetRequiredService<IHostEnvironment>();
            var loggerFactory = sp.GetRequiredService<ILoggerFactory>();
            options.UseLoggerFactory(loggerFactory);

            if (env.IsDevelopment()) {
                options.EnableDetailedErrors();
                options.EnableSensitiveDataLogging();
            }

            options.UseSqlServer(configuration.GetConnectionString("AppDB"), sql => {
                sql.EnableRetryOnFailure(5, TimeSpan.FromSeconds(2), null);
                sql.MigrationsHistoryTable("__EFMigrationsHistory", "orders");
                sql.CommandTimeout(30);
            });
        });

        // Authorization policies local to Orders module
        services
            .AddAuthorizationBuilder()
            .AddPolicy(OrdersConstants.Read, p => p.RequireClaim("scope", "orders.read"))
            .AddPolicy(OrdersConstants.Write, p => p.RequireClaim("scope", "orders.write"));

        return services;
    }

    public static IEndpointRouteBuilder MapOrders(this WebApplication app) {
        if (app.Environment.IsDevelopment()) {
            using var scope = app.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<OrdersDbContext>();
            db.Database.Migrate();
        }

        return app.MapOrdersEndpoints();
    }
}