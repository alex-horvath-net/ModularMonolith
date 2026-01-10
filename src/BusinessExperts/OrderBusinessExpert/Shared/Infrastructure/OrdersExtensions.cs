using Experts.OrderBusinessExpert.GetAllOrderBusinessWorkFlow;
using Experts.OrderBusinessExpert.GetOrderByIdBusinessWorkFlow;
using Experts.OrderBusinessExpert.PlaceOrderBusinessWorkFlow;
using Experts.OrderBusinessExpert.PlaceOrderBusinessWorkFlow.Shared.Infrastructure;
using Experts.OrderBusinessExpert.Shared.Business;
using Experts.OrderBusinessExpert.Shared.Infrastructure.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Experts.OrderBusinessExpert.Shared.Infrastructure;

public static class OrdersExtensions {
    public static IServiceCollection AddOrders(this IServiceCollection services, IConfiguration configuration) {

        // Application
        services.AddScoped<GetAllOrderQueryHandler>();
        services.AddScoped<GetOrderQueryHandler>();
        services.AddPlaceOrderBusinessWorkFlow(configuration);

        // Infrastructure
        services.AddDbContext<OrdersDbContext>((sp, options) => {
            var env = sp.GetRequiredService<IHostEnvironment>();
            var loggerFactory = sp.GetRequiredService<ILoggerFactory>();
            options.UseLoggerFactory(loggerFactory);
            options.ConfigureWarnings(w => w.Ignore(RelationalEventId.PendingModelChangesWarning));

            if (env.IsDevelopment()) {
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
        using var scope = app.Services.CreateScope();
        
        var db = scope.ServiceProvider.GetRequiredService<OrdersDbContext>();
        if (app.Environment.IsEnvironment("IntegrationTest")) {
            //db.Database.EnsureDeleted();
            db.Database.EnsureCreated();
            CleanDatabase(db);
        } else {
            db.Database.Migrate();
        }

        app.MapOrdersEndpoints();
        app.MapCreateOrderEndpoint();

        return app;
    }

    private static void CleanDatabase(DbContext db) {
        foreach (var entityType in db.Model.GetEntityTypes()) {
            var tableName = entityType.GetTableName();
            var schema = entityType.GetSchema();
            if (tableName is null) {
                continue;
            }

            var fullName = string.IsNullOrWhiteSpace(schema)
                ? $"[{tableName}]"
                : $"[{schema}].[{tableName}]";

            db.Database.ExecuteSqlRaw($"DELETE FROM {fullName}");
        }
    }
}