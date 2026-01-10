using Common.Events;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore.Metadata;
using Business.Experts.BillingBusinessExpert.Infrastructure.Data;
using Business.Experts.BillingBusinessExpert.GetInvoice.API;
using Business.Experts.BillingBusinessExpert.GetInvoice.QueryHandlers;
using Business.Experts.BillingBusinessExpert.CreateInvoice;
using Business.Experts.Shared.Business.Events;

namespace Business.Experts.BillingBusinessExpert;

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
            options.ConfigureWarnings(w => w.Ignore(RelationalEventId.PendingModelChangesWarning));

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
        if (app.Environment.IsEnvironment("IntegrationTest")) {
            db.Database.EnsureCreated();
            CleanDatabase(db);
        } else {
            db.Database.Migrate();
        }

        return BillingEndpoints.MapBilling(app);
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
