using System.Net.Http.Json;
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

public class CreateOrderEndpointDirectTests : IAsyncLifetime {
    private readonly MsSqlContainer _dbContainer = new MsSqlBuilder()
        .WithImage("mcr.microsoft.com/mssql/server:2022-latest")
        .WithPassword("Strong_password_123!")
        .Build();

    private HttpClient? _client;

    public async Task InitializeAsync() {
        await _dbContainer.StartAsync();

        var builder = WebApplication.CreateBuilder(new WebApplicationOptions {
            EnvironmentName = "IntegrationTest"
        });

        builder.WebHost.UseTestServer();

        builder.Services.AddDbContext<OrdersDbContext>(options =>
            options.UseSqlServer(_dbContainer.GetConnectionString()));
        builder.Services.AddScoped<CreateOrderCommandHandler>();
        builder.Services.AddScoped<IValidator<CreateOrderCommand>, CreateOrderCommandValidator>();
        builder.Services.AddScoped<IBusinessEventPublisher, InProcessBusinessEventPublisher>();
        builder.Services.AddScoped<IBusinessEventHandler<OrderPlaced>, NoOpOrderPlacedHandler>();
        builder.Services.AddApiVersioning(options => options.ReportApiVersions = true).AddApiExplorer();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddAuthorization(options => {
            options.AddPolicy(OrdersConstants.Write, policy => policy.RequireAssertion(_ => true));
            options.DefaultPolicy = options.GetPolicy(OrdersConstants.Write)!;
            options.FallbackPolicy = options.DefaultPolicy;
        });

        var app = builder.Build();

        using (var scope = app.Services.CreateScope()) {
            var db = scope.ServiceProvider.GetRequiredService<OrdersDbContext>();
            await db.Database.EnsureCreatedAsync();
        }

        app.UseAuthorization();
        app.MapCreateOrderEndpoint();

        await app.StartAsync();

        _client = app.GetTestClient();
    }

    public async Task DisposeAsync() {
        if (_client is IAsyncDisposable asyncDisposable) {
            await asyncDisposable.DisposeAsync();
        }

        await _dbContainer.StopAsync();
    }

    [Fact]
    public async Task PostOrders_ShouldCreateOrder() {
        var command = new CreateOrderCommand(
            CustomerId: Guid.NewGuid(),
            Lines: [new OrderLineRequest(Guid.NewGuid(), 1, 10.0m)]);

        var response = await _client!.PostAsJsonAsync("/v1/orders", command);

        response.EnsureSuccessStatusCode();
    }

    private sealed class NoOpOrderPlacedHandler : IBusinessEventHandler<OrderPlaced> {
        public Task Handle(OrderPlaced businessEvent, CancellationToken token = default) => Task.CompletedTask;
    }
}
