using BusinessExperts.Orders.Featrures.Create;
using BusinessExperts.Orders.Featrures.Create.Infrastructure.Data;
using FluentAssertions;

namespace DevTests.IntegrationTests;

public class CreateOrderCommandHandlerTests : BaseIntegrationTest {

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
        var db = Get<OrdersDbContext>();
        // Act
        var id = await handler.Handle(command, default);

        // Assert 
        var product = db.Orders.FirstOrDefault(p => p.Id == id);

        product.Should().NotBeNull(); 
    }
}