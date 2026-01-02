using BusinessExperts.Orders.Contracts.Events;
using BusinessExperts.Orders.Featrures.Create;
using BusinessExperts.Orders.Featrures.Create.Infrastructure.Data;
using Common.Events;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Testcontainers.MsSql;

namespace DevTests.IntegrationTests;

public abstract partial class BaseIntegrationTest : IAsyncLifetime, IDisposable {
    private readonly MsSqlContainer _dbContainer = new MsSqlBuilder()
        .WithImage("mcr.microsoft.com/mssql/server:2022-latest")
        .WithPassword("Strong_password_123!")
        .Build();

    private ServiceProvider? _provider;
    private IServiceScope? _scope;
    protected CreateOrderCommandHandler Handler = default!;
    protected OrdersDbContext OrdersDB = default!;

    public async Task InitializeAsync() {
        await _dbContainer.StartAsync();

        var services = new ServiceCollection();
        services.AddDbContext<OrdersDbContext>(options =>
            options.UseSqlServer(_dbContainer.GetConnectionString()));
        services.AddScoped<CreateOrderCommandHandler>();
        services.AddScoped<IValidator<CreateOrderCommand>, CreateOrderCommandValidator>();
        services.AddScoped<IBusinessEventPublisher, InProcessBusinessEventPublisher>();
        services.AddScoped<IBusinessEventHandler<OrderPlaced>, NoOpOrderPlacedHandler>();

        _provider = services.BuildServiceProvider();
        _scope = _provider.CreateScope();

        Handler = _scope.ServiceProvider.GetRequiredService<CreateOrderCommandHandler>();
        OrdersDB = _scope.ServiceProvider.GetRequiredService<OrdersDbContext>();
        await OrdersDB.Database.EnsureCreatedAsync();
    }

    public Task DisposeAsync() => _dbContainer.StopAsync();

    public void Dispose() {
        _scope?.Dispose();
        _provider?.Dispose();
    }
}
