using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore.Metadata;
using Business.Experts.OrderBusinessExpert.Shared.Infrastructure.Data;
using Business.Experts.OrderBusinessExpert.WorkFlows.GetAllOrder;
using Business.Experts.OrderBusinessExpert.WorkFlows.PlaceOrderBusinessWorkFlow.Shared.Infrastructure;
using Business.Experts.OrderBusinessExpert.WorkFlows.GetOrderById;
using Business.Experts.OrderBusinessExpert.WorkFlows.PlaceOrderBusinessWorkFlow.WorkSteps;
using Business.Experts.OrderBusinessExpert.WorkFlows.PlaceOrderBusinessWorkFlow;
using Business.Experts.OrderBusinessExpert.WorkFlows.PlaceOrderBusinessWorkFlow.Shared.Business.Domain;

namespace Business.Experts.OrderBusinessExpert;

public static class OrdersExtensions {
    public static IServiceCollection AddOrders(this IServiceCollection services, IConfiguration configuration) {

        // Application
        services.AddScoped<GetAllOrderQueryHandler>();
        services.AddScoped<GetOrderQueryHandler>();
        services.AddScoped<PlaceOrderWorkflow>();
        services.AddScoped<ValidatorWorkStep>();
        services.AddScoped<CreateOrderWorkStep>();
        services.AddScoped<PersistWorkStep>();
        services.AddScoped<PublishWorkStep>();

        // Register validator so Minimal APIs resolve it from DI (not Body)
        services.AddScoped<IValidator<CreateOrderRequest>, CreateOrderRequestValidator>();

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