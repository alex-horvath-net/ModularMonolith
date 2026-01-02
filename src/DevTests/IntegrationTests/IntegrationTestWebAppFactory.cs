using BusinessExperts.Billing.Infrastructure.Data;
using BusinessExperts.Contracts.Events;
using BusinessExperts.Orders.Featrures.Create.Infrastructure.Data;
using Common.Events;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Testcontainers.MsSql;

namespace DevTests.IntegrationTests;

public class IntegrationTestWebAppFactory
    : WebApplicationFactory<global::Program>,
      IAsyncLifetime {
    private readonly MsSqlContainer _dbContainer = new MsSqlBuilder()
        .WithImage("mcr.microsoft.com/mssql/server:2022-latest")
        .WithPassword("Strong_password_123!")
        .Build();

    protected override void ConfigureWebHost(IWebHostBuilder builder) {
        builder.UseEnvironment("IntegrationTest");

        builder.ConfigureAppConfiguration((_, config) => {
            config.AddInMemoryCollection(new Dictionary<string, string?> {
                ["Authentication:Issuer"] = "integration-tests",
                ["Authentication:Audience"] = "integration-tests",
                ["Authentication:SecurityKey"] = "integration-test-key-012345678901234567890123456789",
                ["Authentication:AllowDevSymmetricKey"] = "true",
                ["Authentication:DevScopes:0"] = "orders.read",
                ["Authentication:DevScopes:1"] = "orders.write"
            });
        });

        builder.ConfigureTestServices(services => {
            var ordersDescriptor = services.SingleOrDefault(s => s.ServiceType == typeof(DbContextOptions<OrdersDbContext>));
            if (ordersDescriptor is not null) {
                services.Remove(ordersDescriptor);
            }

            services.AddDbContext<OrdersDbContext>(options =>
                options.UseSqlServer(_dbContainer.GetConnectionString()));

            var billingDescriptor = services.SingleOrDefault(s => s.ServiceType == typeof(DbContextOptions<BillingDbContext>));
            if (billingDescriptor is not null) {
                services.Remove(billingDescriptor);
            }

            var billingHandlerDescriptor = services.SingleOrDefault(s => s.ServiceType == typeof(IBusinessEventHandler<OrderPlaced>));
            if (billingHandlerDescriptor is not null) {
                services.Remove(billingHandlerDescriptor);
            }

            services.AddScoped<IBusinessEventHandler<OrderPlaced>, NoOpOrderPlacedHandler>();
        });
    }

    public async Task InitializeAsync() {
        await _dbContainer.StartAsync();

        using var scope = Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<OrdersDbContext>();
        await db.Database.EnsureCreatedAsync();
    }

    public new Task DisposeAsync() => _dbContainer.StopAsync();

    private sealed class NoOpOrderPlacedHandler : IBusinessEventHandler<OrderPlaced> {
        public Task Handle(OrderPlaced businessEvent, CancellationToken token = default) => Task.CompletedTask;
    }
}
