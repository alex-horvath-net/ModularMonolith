using Billing.API;
using Billing.Application.EventHandlers;
using Billing.Application.QueryHandlers;
using Billing.Infrastructure.Data;
using Common;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Orders.Contracts.Events;

namespace Billing;

public static class Extensions
{
    public static IServiceCollection AddBilling(this IServiceCollection services, IConfiguration configuration)
    {
        // Application
        services.AddScoped<GetInvoiceQueryHandler>();
        services.AddScoped<IBusinessEventHandler<OrderPlaced>, OrderPlacedEventHandler>();

        //Infrastructure
        services.AddDbContext<BillingDbContext>((sp, options) =>
        {
            var env = sp.GetRequiredService<IHostEnvironment>();
            var loggerFactory = sp.GetRequiredService<ILoggerFactory>();
            options.UseLoggerFactory(loggerFactory);

            if (env.IsDevelopment())
            {
                options.EnableDetailedErrors();
                options.EnableSensitiveDataLogging();
            }

            options.UseSqlServer(configuration.GetConnectionString("AppDB"), sql =>
            {
                sql.EnableRetryOnFailure(5, TimeSpan.FromSeconds(2), null);
                sql.MigrationsHistoryTable("__EFMigrationsHistory", "billing");
                sql.CommandTimeout(30);
            });
        });

        return services;
    }

    public static IEndpointRouteBuilder MapBilling(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            using var scope = app.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<BillingDbContext>();
            db.Database.Migrate();
        }

        return BillingEndpoints.MapBilling(app);
    }
}
