using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Orders.API;
using Orders.Application.CommandHandlers;
using Orders.Application.QueryHandlers;
using Orders.Infrastructure.Data;

namespace Orders;

public static class Extensions {
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

            if (env.IsDevelopment()) {
                options.EnableDetailedErrors();
                options.EnableSensitiveDataLogging();
            }

            options.UseSqlServer(configuration.GetConnectionString("OrdersDB"), sql => {
                sql.EnableRetryOnFailure(5, TimeSpan.FromSeconds(2), null);
                sql.CommandTimeout(30);
            });
        });

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