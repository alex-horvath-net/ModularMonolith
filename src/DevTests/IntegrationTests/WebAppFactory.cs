using Experts.BillingBusinessExpert.Infrastructure.Data;
using Experts.OrderBusinessExpert.Shared.Infrastructure.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace DevTests.IntegrationTests;

public class WebAppFactory : WebApplicationFactory<Program>, IAsyncLifetime {

    private IServiceScope _scope = default!;

    public T GetRequiredService<T>() where T : notnull => _scope.ServiceProvider.GetRequiredService<T>();


    protected override void ConfigureWebHost(IWebHostBuilder builder) {
        builder.UseSetting(WebHostDefaults.EnvironmentKey, "IntegrationTest");
        builder.UseTestServer();
        builder.ConfigureAppConfiguration((_, configBuilder) => {
            configBuilder.AddInMemoryCollection(new Dictionary<string, string?> {
                ["ConnectionStrings:AppDB"] = "Server=.\\SQLEXPRESS;Database=AppDb_Test;User ID=sa;Password=Dev!Pass123;TrustServerCertificate=True;Encrypt=False",
                ["Authentication:Issuer"] = "integration-tests",
                ["Authentication:Audience"] = "integration-tests",
                ["Authentication:SecurityKey"] = "integration-test-key-012345678901234567890123456789",
                ["Authentication:AllowDevSymmetricKey"] = "true",
                ["Authentication:DevScopes:0"] = "orders.read",
                ["Authentication:DevScopes:1"] = "orders.write",
                ["Authentication:DevScopes:2"] = "billing.read"
            });
        });
    }
    
    public async Task InitializeAsync() {
        _scope = Services.CreateScope();

        var ordersDb = GetRequiredService<OrdersDbContext>();
        await ordersDb.Database.EnsureCreatedAsync();

        var billingDb = GetRequiredService<BillingDbContext>();
        await billingDb.Database.EnsureCreatedAsync();
    }

    public new Task DisposeAsync() {
        _scope?.Dispose();
        return Task.CompletedTask;
    }
}

