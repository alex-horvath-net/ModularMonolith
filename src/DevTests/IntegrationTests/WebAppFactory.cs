using Business.MemberApplicationUser.BillingBusinessExpert.Infrastructure.Data;
using Business.MemberApplicationUser.OrderBusinessExpert.CreateOrderWorkFlow.Infrastructure.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Testcontainers.MsSql;

namespace DevTests.IntegrationTests;

public class WebAppFactory : WebApplicationFactory<Program>, IAsyncLifetime {

    private readonly MsSqlContainer _dbContainer = new MsSqlBuilder("mcr.microsoft.com/mssql/server:2022-latest")
        .WithPassword("Strong_password_123!")
        .Build();

    private IServiceScope _scope = default!;

    public T GetRequiredService<T>() where T : notnull => _scope.ServiceProvider.GetRequiredService<T>();


    protected override void ConfigureWebHost(IWebHostBuilder builder) {
        builder.UseSetting(WebHostDefaults.EnvironmentKey, "IntegrationTest");
        builder.UseTestServer();
        builder.ConfigureAppConfiguration((_, configBuilder) => {
            configBuilder.AddInMemoryCollection(new Dictionary<string, string?> {
                ["ConnectionStrings:AppDB"] = _dbContainer.GetConnectionString(),
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
            UseTestContainerDB<OrdersDbContext>(services);
            UseTestContainerDB<BillingDbContext>(services);
        });
    }
    
    public async Task InitializeAsync() {
        await _dbContainer.StartAsync();

        _scope = base.Services.CreateScope();
        var ordersDb = GetRequiredService<OrdersDbContext>();
        await ordersDb.Database.MigrateAsync();

        var billingDb = GetRequiredService<BillingDbContext>();
        await billingDb.Database.MigrateAsync();
    }

    public new async Task DisposeAsync() {
        _scope?.Dispose();
        await _dbContainer.StopAsync();
    }

    private void UseTestContainerDB<T>(IServiceCollection services) where T : DbContext {
        var descriptorType = typeof(DbContextOptions<T>);
        var descriptor = services.SingleOrDefault(s => s.ServiceType == descriptorType);
        if (descriptor is not null) {
            services.Remove(descriptor);
        }

        services.AddDbContext<T>(options => {
            options.UseSqlServer(_dbContainer.GetConnectionString());
            options.ConfigureWarnings(w => w.Ignore(RelationalEventId.PendingModelChangesWarning));
        });
    }
}

