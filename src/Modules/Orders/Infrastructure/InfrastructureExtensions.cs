using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Orders.Infrastructure;

public static class InfrastructureExtensions {
    public static IServiceCollection AddOrdersInfrastructure(this IServiceCollection services, IConfiguration configuration) {
        services.AddDbContext<OrdersDbContext>(options =>

        options
            .EnableDetailedErrors()
            .EnableSensitiveDataLogging()
            .UseLoggerFactory(LoggerFactory.Create(builder => builder.AddConfiguration(configuration)))
            .UseSqlServer(configuration.GetConnectionString("OrdersDB"), sqlOptions => sqlOptions.EnableRetryOnFailure())
        );
        return services;
    }
}
