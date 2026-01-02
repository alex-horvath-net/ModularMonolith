using BusinessExperts.Orders.Featrures.Create;
using BusinessExperts.Orders.Featrures.Create.Infrastructure.Data;
using Microsoft.Extensions.DependencyInjection;

namespace DevTests.IntegrationTests;

public abstract class BaseIntegrationTest : IClassFixture<IntegrationTestWebAppFactory>, IDisposable {

    private readonly IServiceScope _scope;
    protected readonly CreateOrderCommandHandler Handler;
    protected readonly OrdersDbContext OrdersDB;

    protected BaseIntegrationTest(IntegrationTestWebAppFactory factory) {
        _scope = factory.Services.CreateScope();

        Handler = _scope.ServiceProvider.GetRequiredService<CreateOrderCommandHandler>();

        OrdersDB = _scope.ServiceProvider.GetRequiredService<OrdersDbContext>();
    }

    public void Dispose() {
        _scope?.Dispose();
        OrdersDB?.Dispose();
    }
}
