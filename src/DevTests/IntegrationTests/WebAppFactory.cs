using System.Linq;
using Experts.BillingBusinessExpert.Infrastructure.Data;
using Experts.OrderBusinessExpert.Shared.Infrastructure.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DevTests.IntegrationTests;

public class WebAppFactory : WebApplicationFactory<Program>, IAsyncLifetime {

    private IServiceScope _scope = default!;

    public T GetRequiredService<T>() where T : notnull => _scope.ServiceProvider.GetRequiredService<T>();


    protected override void ConfigureWebHost(IWebHostBuilder builder) {
        builder.UseSetting(WebHostDefaults.EnvironmentKey, "IntegrationTest");
        builder.UseTestServer();
        builder.ConfigureAppConfiguration((_, configBuilder) => {
            configBuilder.AddInMemoryCollection(new Dictionary<string, string?> {
                ["ConnectionStrings:AppDB"] = "InMemory-DevTests",
                ["Authentication:Issuer"] = "integration-tests",
                ["Authentication:Audience"] = "integration-tests",
                ["Authentication:SecurityKey"] = "integration-test-key-012345678901234567890123456789",
                ["Authentication:AllowDevSymmetricKey"] = "true",
                ["Authentication:DevScopes:0"] = "orders.read",
                ["Authentication:DevScopes:1"] = "orders.write",
                ["Authentication:DevScopes:2"] = "billing.read"
            });
        });
        builder.ConfigureTestServices(services => {
            RemoveSqlServerProvider(services);
            UseTestDatabase<OrdersDbContext>(services, "orders-test-db");
            UseTestDatabase<BillingDbContext>(services, "billing-test-db");
        });
    }
    
    public Task InitializeAsync() {
        _scope = Services.CreateScope();
        return Task.CompletedTask;
    }

    public new Task DisposeAsync() {
        _scope?.Dispose();
        return Task.CompletedTask;
    }

    private void UseTestDatabase<T>(IServiceCollection services, string dbName) where T : DbContext {
        services.RemoveAll(typeof(DbContextOptions<T>));
        services.RemoveAll(typeof(IConfigureOptions<DbContextOptions<T>>));
        services.RemoveAll(typeof(IOptions<DbContextOptions<T>>));
        services.RemoveAll(typeof(T));

        var optionsBuilder = new DbContextOptionsBuilder<T>();
        optionsBuilder.UseInMemoryDatabase(dbName);
        optionsBuilder.ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning));

        services.AddSingleton(optionsBuilder.Options);
        services.AddScoped(sp => {
            var loggerFactory = sp.GetRequiredService<ILoggerFactory>();
            optionsBuilder.UseLoggerFactory(loggerFactory);
            return (T)ActivatorUtilities.CreateInstance(sp, typeof(T), optionsBuilder.Options);
        });
    }

    private static void RemoveSqlServerProvider(IServiceCollection services) {
        var providerDescriptors = services
            .Where(d =>
                d.ServiceType?.Namespace?.Contains("Microsoft.EntityFrameworkCore.SqlServer", StringComparison.Ordinal) == true ||
                d.ImplementationType?.Namespace?.Contains("Microsoft.EntityFrameworkCore.SqlServer", StringComparison.Ordinal) == true ||
                d.ImplementationInstance?.GetType().Namespace?.Contains("Microsoft.EntityFrameworkCore.SqlServer", StringComparison.Ordinal) == true)
            .ToList();

        foreach (var descriptor in providerDescriptors) {
            services.Remove(descriptor);
        }
    }
}

