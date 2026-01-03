using BusinessExperts.Orders.Featrures.Create;
using BusinessExperts.Orders.Featrures.Create.Infrastructure.Data;
using FluentAssertions;

namespace DevTests.IntegrationTests;

public class CreateOrderCommandHandlerTests(WebAppFactory factory) : IClassFixture<WebAppFactory> {

    [Fact]
    public async Task CreateOrderCommandHandler_ShouldCreateOrder() {
        // Arrange
        var commandHandler = factory.GetRequiredService<CreateOrderCommandHandler>();
        var command = new CreateOrderCommand(
            CustomerId: Guid.NewGuid(),
            Lines: [
                new OrderLineRequest( ProductId: Guid.NewGuid(), Quantity: 1, UnitPrice: 10.0m )
            ]
        );

        // Act
        var id = await commandHandler.Handle(command, CancellationToken.None);

        // Assert 
        var db = factory.GetRequiredService<OrdersDbContext>();
        var order = db.Orders.FirstOrDefault(p => p.Id == id);
        order.Should().NotBeNull();
    }
}