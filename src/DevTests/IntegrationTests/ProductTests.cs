using BusinessExperts.Orders.Featrures.Create;
using FluentAssertions;
namespace DevTests.IntegrationTests;

public class ProductTests(IntegrationTestWebAppFactory factory) : BaseIntegrationTest(factory) {

    [Fact]
    public async Task Create_ShouldCreateProduct() {
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