using BusinessExperts.Orders.Featrures.Create;
using FluentAssertions;
using Xunit;

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

        // Act
        var id = await Handler.Handle(command, default);

        // Assert 
        var product = OrdersDB.Orders.FirstOrDefault(p => p.Id == id);

        product.Should().NotBeNull(); 
    }
}