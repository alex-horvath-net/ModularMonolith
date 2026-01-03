using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using BusinessExperts.Identity.CreateToken;
using BusinessExperts.Orders.Featrures.Create;
using FluentAssertions;

namespace DevTests.IntegrationTests;

public class CreateOrderEndpoint_Tests(WebAppFactory factory) : IClassFixture<WebAppFactory> {

    [Fact]
    public async Task PostOrders_ShouldCreateOrder() {

        // Arrange
        var client = factory.CreateClient();
        var tokenHandler = factory.GetRequiredService<CreateTokenCommandHandler>();
        var token = await tokenHandler.Handle(new CreateTokenCommand());
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var command = new CreateOrderCommand(
            CustomerId: Guid.NewGuid(),
            Lines: [new OrderLineRequest(Guid.NewGuid(), 1, 10.0m)]);

        // Act
        var response = await client.PostAsJsonAsync("/v1/orders", command);

        // Assert
        response.EnsureSuccessStatusCode();
        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }
}
