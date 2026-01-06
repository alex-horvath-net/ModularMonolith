using BusinessExperts.Order.CreateOrderWorkFlow;
using BusinessExperts.Order.CreateOrderWorkFlow.Infrastructure.Data;
using FluentAssertions;

namespace DevTests.IntegrationTests;

public class CreateOrder_WorkFlow_Tests(WebAppFactory factory) : IClassFixture<WebAppFactory> {

    [Fact]
    public async Task CreateOrderCommandHandler_ShouldCreateOrder() {
        // Arrange
        var workflow = factory.GetRequiredService<CreateOrderWorkFlow>();
        var request = new CreateOrderRequest(
            CustomerId: Guid.NewGuid(),
            Lines: [
                new CreateOrderLineRequest( ProductId: Guid.NewGuid(), Quantity: 1, UnitPrice: 10.0m )
            ]
        );

        // Act
        var response = await workflow.Run(request, CancellationToken.None);

        // Assert  
        var db = factory.GetRequiredService<OrdersDbContext>();
        var order_in_db = db.Orders.FirstOrDefault(p => p.Id == response.Order.Id);
        order_in_db.Should().NotBeNull();
    }
}