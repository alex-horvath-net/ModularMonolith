using BusinessExperts.Billing.CreateInvoice;
using BusinessExperts.Billing.Infrastructure.Data;
using BusinessExperts.Contracts.Events;
using BusinessExperts.Orders;
using BusinessExperts.Orders.Featrures.Create;
using BusinessExperts.Orders.Featrures.Create.Infrastructure.Data;
using Common.Events;
using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Testcontainers.MsSql;

namespace DevTests.IntegrationTests;

public abstract class BaseIntegrationTest : IAsyncLifetime {

    private readonly MsSqlContainer _dbContainer = new MsSqlBuilder()
        .WithImage("mcr.microsoft.com/mssql/server:2022-latest")
        .WithPassword("Strong_password_123!")
        .Build();

    private IServiceScope _scope = default!;
    protected HttpClient TestClient = default!;

    public async Task InitializeAsync() {
        await _dbContainer.StartAsync();

        var builder = WebApplication.CreateBuilder(new WebApplicationOptions {
            EnvironmentName = "IntegrationTest"
        });

        builder.WebHost.UseTestServer();

        builder.Services.AddDbContext<OrdersDbContext>(options => options.UseSqlServer(_dbContainer.GetConnectionString()));
        builder.Services.AddDbContext<BillingDbContext>(options => options.UseSqlServer(_dbContainer.GetConnectionString()));
        builder.Services.AddScoped<CreateOrderCommandHandler>();
        builder.Services.AddScoped<IValidator<CreateOrderCommand>, CreateOrderCommandValidator>();
        builder.Services.AddScoped<IBusinessEventPublisher, InProcessBusinessEventPublisher>();
        builder.Services.AddScoped<IBusinessEventHandler<OrderPlaced>, OrderPlacedEventHandler>();
        builder.Services.AddApiVersioning(options => options.ReportApiVersions = true).AddApiExplorer();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddAuthorization(options => {
            options.AddPolicy(OrdersConstants.Write, policy => policy.RequireAssertion(_ => true));
            options.DefaultPolicy = options.GetPolicy(OrdersConstants.Write)!;
            options.FallbackPolicy = options.DefaultPolicy;
        });

        var app = builder.Build();

        _scope = app.Services.CreateScope();
        var orderDB = _scope.ServiceProvider.GetRequiredService<OrdersDbContext>();
        await orderDB.Database.EnsureCreatedAsync();

        var billingDB = _scope.ServiceProvider.GetRequiredService<BillingDbContext>();
        await billingDB.Database.EnsureCreatedAsync();


        app.UseAuthorization();
        app.MapCreateOrderEndpoint();


        await app.StartAsync();

        TestClient = app.GetTestClient();
    }

    public T Get<T>() where T : notnull {
        return _scope.ServiceProvider.GetRequiredService<T>();
    }

    public async Task DisposeAsync() {
        if (TestClient is IAsyncDisposable asyncDisposable) {
            await asyncDisposable.DisposeAsync();
        }

        await _dbContainer.StopAsync();

        _scope?.Dispose();
    }

}
