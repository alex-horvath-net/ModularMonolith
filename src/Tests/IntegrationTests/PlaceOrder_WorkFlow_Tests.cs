//using Experts.OrderExpert.PlaceOrder;
//using Experts.OrderExpert.Shared.Infrastructure.Data;
//using FluentAssertions;

//namespace Tests.IntegrationTests;

//public class PlaceOrder_WorkFlow_Tests(WebAppFactory factory) : IClassFixture<WebAppFactory> {

//    [Fact]
//    public async Task CreateOrderCommandHandler_ShouldCreateOrder() {
//        // Arrange
//        var workflow = factory.GetRequiredService<BusinessWorkFlow>();
//        var request = new PlaceOrderRequest(
//            CustomerId: Guid.NewGuid(),
//            Lines: [
//                new PlaceOrderLineRequest( ProductId: Guid.NewGuid(), Quantity: 1, UnitPrice: 100.0m )
//            ]
//        );

//        // Act
//        var response = await workflow.Run(request, CancellationToken.None);

//        // Assert  
//        var db = factory.GetRequiredService<OrdersDbContext>();
//        var order_in_db = db.Orders.FirstOrDefault(p => p.Id == response.Order.Id);
//        order_in_db.Should().NotBeNull();
//    }
//}