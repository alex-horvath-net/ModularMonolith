using Business.ApplicationUsers.Member.Billing.CreateInvoice;
using Business.ApplicationUsers.Member.Billing.GetInvoice.API;
using Business.ApplicationUsers.Member.Billing.GetInvoice.QueryHandlers;
using Business.ApplicationUsers.Member.Billing.Infrastructure.Data;
using Business.ApplicationUsers.Member.Contracts.Events;
using Common.Events;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Business.ApplicationUsers.Member.Billing;

public static class BillingExtensions
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

        // Authorization policies local to Billing module
        services
            .AddAuthorizationBuilder()
            .AddPolicy("Billing.Read", p => p.RequireClaim("scope", "billing.read"));

        return services;
    }

    public static IEndpointRouteBuilder MapBilling(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<BillingDbContext>();
        db.Database.Migrate();

        return BillingEndpoints.MapBilling(app);
    }
}
