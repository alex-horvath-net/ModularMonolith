using Billing.API;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Billing;

public static class BillingModuleExtensions {
    public static IServiceCollection AddBilling(this IServiceCollection services, IConfiguration configuration) {
        services.AddBillingInfrastructure(configuration)
        .AddBillingApplication();
        return services;
    }

    public static IEndpointRouteBuilder MapBilling(this WebApplication app) {
        // Ensure databases exist (development-friendly). For production, prefer migrations.
        using (var scope = app.Services.CreateScope()) {
            var sp = scope.ServiceProvider;

            var billingDb = sp.GetRequiredService<BillingDbContext>();
            billingDb.Database.EnsureCreated();
        }

        return BillingEndpoints.MapBilling(app);
    }
}
