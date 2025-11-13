using Billing.API;
using Billing.Infrastructure;
using Billing.Infrastructure.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Billing;

public static class BillingModuleExtensions {
    public static IServiceCollection AddBilling(this IServiceCollection services, IConfiguration configuration) {
        services.AddBillingInfrastructure(configuration)
        .AddBillingApplication();
        return services;
    }

    public static IEndpointRouteBuilder MapBilling(this WebApplication app) {
        if (app.Environment.IsDevelopment()) {
            using var scope = app.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<BillingDbContext>();
            db.Database.Migrate();
        }

        return BillingEndpoints.MapBilling(app);
    }
}
