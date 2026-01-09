using Common.Events;
using Experts.BillingBusinessExpert.CreateInvoice;
using Experts.BillingBusinessExpert.GetInvoice.API;
using Experts.BillingBusinessExpert.GetInvoice.QueryHandlers;
using Experts.BillingBusinessExpert.Infrastructure.Data;
using Experts.Shared.Business.Events;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Experts.BillingBusinessExpert;

public static class BillingExtensions {
    public static IServiceCollection AddBilling(this IServiceCollection services, IConfiguration configuration) {
        // Application
        services.AddScoped<GetInvoiceQueryHandler>();
        services.AddScoped<IBusinessEventHandler<OrderPlaced>, OrderPlacedEventHandler>();

        //Infrastructure
        services.AddDbContext<BillingDbContext>((sp, options) => {
            var env = sp.GetRequiredService<IHostEnvironment>();
            var loggerFactory = sp.GetRequiredService<ILoggerFactory>();
            options.UseLoggerFactory(loggerFactory);

            if (env.IsDevelopment()) {
                options.EnableDetailedErrors();
                options.EnableSensitiveDataLogging();
            }

            options.UseSqlServer(configuration.GetConnectionString("AppDB"), sql => {
                sql.EnableRetryOnFailure(5, TimeSpan.FromSeconds(2), null);
                sql.MigrationsHistoryTable("__EFMigrationsHistory", "billing");
                sql.CommandTimeout(30);
            });
        });

        // Authorization policies local to Billing module
        services
            .AddAuthorizationBuilder()
            .AddPolicy("Billing.Read", p => p.RequireClaim("scope", "billing.read"));

        return services;
    }

    public static IEndpointRouteBuilder MapBilling(this WebApplication app) {
        using var scope = app.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<BillingDbContext>();
        db.Database.EnsureCreated();
        db.Database.Migrate();

        return BillingEndpoints.MapBilling(app);
    }
}
