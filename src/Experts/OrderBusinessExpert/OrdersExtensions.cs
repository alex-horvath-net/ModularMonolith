using Experts.OrderBusinessExpert.Shared.Infrastructure.Data;
using Experts.OrderBusinessExpert.WorkFlows.GetAllOrder;
using Experts.OrderBusinessExpert.WorkFlows.GetOrderById;
using Experts.OrderBusinessExpert.WorkFlows.PlaceOrderBusinessWorkFlow.Shared.Business.Domain;
using Experts.OrderBusinessExpert.WorkFlows.PlaceOrderBusinessWorkFlow.Shared.Infrastructure;
using Experts.OrderBusinessExpert.WorkFlows.PlaceOrderBusinessWorkFlow.WorkSteps;
using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Experts.OrderBusinessExpert;

public static class OrdersExtensions {
    public static IServiceCollection AddOrders(this IServiceCollection services, IConfiguration configuration) {

        // Application
        services.AddScoped<GetAllOrderQueryHandler>();
        services.AddScoped<GetOrderQueryHandler>();
        services.AddScoped<WorkFlows.PlaceOrderBusinessWorkFlow.PlaceOrderWorkflow>();
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
        db.Database.EnsureCreated();
        db.Database.Migrate();

        app.MapOrdersEndpoints();
        app.MapCreateOrderEndpoint();

        return app;
    }
}