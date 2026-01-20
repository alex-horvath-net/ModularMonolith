//using System.Net;
//using System.Net.Http.Json;
//using Experts.OrderExpert.PlaceOrder;
//using FluentAssertions;

//namespace Tests.IntegrationTests;

//public class PlaceOrder_Endpoint_Tests(WebAppFactory applicationApi) : IClassFixture<WebAppFactory> {

//    [Fact]
//    public async Task PostOrders_ShouldCreateOrder() {

//        // Arrange
//        var placeOrderRequest = new PlaceOrderRequest(
//                    CustomerId: Guid.NewGuid(),
//                    Lines: [new PlaceOrderLineRequest(Guid.NewGuid(), 1, 100.0m)]);

//        // Act
//        var response = await applicationApi.Post("/v1/orders", placeOrderRequest);

//        // Assert
//        response.EnsureSuccessStatusCode();
//        response.StatusCode.Should().Be(HttpStatusCode.Created);
//        response.Version.ToString().Should().Be("1.1");
//        response.Headers.GetValues("api-supported-versions").First().Should().Be("1.0");
//        response.Headers.Location?.OriginalString.Should().StartWith("/v1/orders/");

//        var content = await response.Content.ReadFromJsonAsync<Guid>();
//        content.Should().NotBe(Guid.Empty);
//    }
//}
