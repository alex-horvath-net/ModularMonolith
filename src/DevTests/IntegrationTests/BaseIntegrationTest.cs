using BusinessExperts.Billing;
using BusinessExperts.Contracts.Events;
using BusinessExperts.Identity.CreateToken;
using BusinessExperts.Orders;
using BusinessExperts.Orders.Featrures.Create.Infrastructure.Data;
using Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Testcontainers.MsSql;
using Asp.Versioning;

namespace DevTests.IntegrationTests;

public abstract class BaseIntegrationTest : IAsyncLifetime {
    private readonly MsSqlContainer _dbContainer = new MsSqlBuilder()
        .WithImage("mcr.microsoft.com/mssql/server:2022-latest")
        .WithPassword("Strong_password_123!")
        .Build();

    private IServiceScope? _scope;
    protected HttpClient TestClient = default!;

    public async Task InitializeAsync() {
        await _dbContainer.StartAsync();

        var builder = WebApplication.CreateBuilder(new WebApplicationOptions {
            EnvironmentName = "IntegrationTest"
        });

        builder.WebHost.UseTestServer();

        builder.Configuration.AddInMemoryCollection(new Dictionary<string, string?> {
            ["ConnectionStrings:AppDB"] = _dbContainer.GetConnectionString(),
            ["Authentication:Issuer"] = "integration-tests",
            ["Authentication:Audience"] = "integration-tests",
            ["Authentication:SecurityKey"] = "integration-test-key-012345678901234567890123456789",
            ["Authentication:AllowDevSymmetricKey"] = "true",
            ["Authentication:DevScopes:0"] = "orders.read",
            ["Authentication:DevScopes:1"] = "orders.write",
            ["Authentication:DevScopes:2"] = "billing.read"
        });

        builder.Services.AddCommon(builder.Configuration, builder.Environment);
        builder.Services.AddOrders(builder.Configuration);
        builder.Services.AddBilling(builder.Configuration);

        builder.Services.AddDbContext<OrdersDbContext>((sp, options) => {
            options.UseSqlServer(_dbContainer.GetConnectionString());
            options.ConfigureWarnings(w => w.Ignore(RelationalEventId.PendingModelChangesWarning));
        });
        builder.Services.AddDbContext<BusinessExperts.Billing.Infrastructure.Data.BillingDbContext>((sp, options) => {
            options.UseSqlServer(_dbContainer.GetConnectionString());
            options.ConfigureWarnings(w => w.Ignore(RelationalEventId.PendingModelChangesWarning));
        });

        builder.Services.AddAuthorization();
        builder.Services.AddApiVersioning(options => options.ReportApiVersions = true)
            .AddApiExplorer();

        var app = builder.Build();

        _scope = app.Services.CreateScope();
        _scope.ServiceProvider.GetRequiredService<IAuthorizationPolicyProvider>();

        app.MapCommon();
        app.MapDevToken();
        app.MapOrders();
        app.MapBilling();

        try {
            await app.StartAsync();
        }
        catch (Exception ex) {
            Console.WriteLine(ex);
            throw;
        }

        TestClient = app.GetTestClient();
    }

    public T Get<T>() where T : notnull => _scope!.ServiceProvider.GetRequiredService<T>();

    public async Task DisposeAsync() {
        if (TestClient is IAsyncDisposable asyncDisposable) {
            await asyncDisposable.DisposeAsync();
        }

        _scope?.Dispose();
        await _dbContainer.StopAsync();
    }
}
