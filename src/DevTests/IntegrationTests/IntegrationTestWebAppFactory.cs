using System.Net.Http.Headers;
using BusinessExperts.Billing.Infrastructure.Data;
using BusinessExperts.Identity.CreateToken;
using BusinessExperts.Orders.Featrures.Create.Infrastructure.Data;
using Common.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Testcontainers.MsSql;

namespace DevTests.IntegrationTests;

public class IntegrationTestWebAppFactory : WebApplicationFactory<Program>, IAsyncLifetime {

    private readonly MsSqlContainer _dbContainer = new MsSqlBuilder("mcr.microsoft.com/mssql/server:2022-latest")
        .WithPassword("Strong_password_123!")
        .Build();
    private string token = default!;

    protected override void ConfigureWebHost(IWebHostBuilder builder) {
        builder.UseSetting(WebHostDefaults.EnvironmentKey, "IntegrationTest");
        builder.UseTestServer();
        builder.ConfigureAppConfiguration((_, config) => {
            config.AddInMemoryCollection(new Dictionary<string, string?> {
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

    protected override void ConfigureClient(HttpClient client) {
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }

    public async Task InitializeAsync() {
        await _dbContainer.StartAsync();

        using var scope = Services.CreateScope();
        var ordersDb = scope.ServiceProvider.GetRequiredService<OrdersDbContext>();
        await ordersDb.Database.MigrateAsync();
        var billingDb = scope.ServiceProvider.GetRequiredService<BillingDbContext>();
        await billingDb.Database.MigrateAsync();

        var jwtOptions = scope.ServiceProvider.GetRequiredService<IOptions<JwtOptions>>();
        var tokenHandler = new CreateTokenCommandHandler(jwtOptions);
        token = await tokenHandler.Handle(new CreateTokenCommand());
    }

    public new Task DisposeAsync() {
        return _dbContainer.StopAsync();
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

