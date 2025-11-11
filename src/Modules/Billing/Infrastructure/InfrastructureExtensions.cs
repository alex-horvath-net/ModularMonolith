using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Billing;

public static class BillingInfrastructureExtensions {
    public static IServiceCollection AddBillingInfrastructure(this IServiceCollection services, IConfiguration configuration) {
        services.AddDbContext<BillingDbContext>(options =>

        options
            .EnableDetailedErrors()
            .EnableSensitiveDataLogging()
            .UseLoggerFactory(LoggerFactory.Create(builder => builder.AddConfiguration(configuration)))
            .UseSqlServer(configuration.GetConnectionString("BillingDB"), sqlOptions => sqlOptions.EnableRetryOnFailure())
        );
        return services;
    }
}
