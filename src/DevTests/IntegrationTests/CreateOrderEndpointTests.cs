using System.Net;
using System.Net.Http.Json;
using BusinessExperts.Orders.Featrures.Create;
using FluentAssertions;

namespace DevTests.IntegrationTests;

public class CreateOrderEndpointTests(IntegrationTestWebAppFactory factory) : BaseIntegrationTest(factory) {

    [Fact]
    public async Task PostOrders_ShouldCreateOrder() {

        var command = new CreateOrderCommand(
            CustomerId: Guid.NewGuid(),
            Lines: [new OrderLineRequest(Guid.NewGuid(), 1, 10.0m)]);

        var response = await TestClient.PostAsJsonAsync("/v1/orders", command);

        response.EnsureSuccessStatusCode();
        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }
}
