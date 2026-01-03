using BusinessExperts.Orders.Featrures.Create;
using BusinessExperts.Orders.Featrures.Create.Infrastructure.Data;
using FluentAssertions;

namespace DevTests.IntegrationTests;

public class CreateOrderCommandHandlerTests(IntegrationTestWebAppFactory factory) : BaseIntegrationTest(factory) {

    [Fact]
    public async Task CreateOrderCommandHandler_ShouldCreateOrder() {
        // Arrange
        var command = new CreateOrderCommand(
            CustomerId: Guid.NewGuid(),
            Lines: [
                new OrderLineRequest( ProductId: Guid.NewGuid(), Quantity: 1, UnitPrice: 10.0m                )
            ]
        );
        var handler = Get<CreateOrderCommandHandler>();
        // Act
        var id = await handler.Handle(command, default);

        // Assert 
        var db = Get<OrdersDbContext>();
        var order = db.Orders.FirstOrDefault(p => p.Id == id);
        order.Should().NotBeNull(); 
    }
}