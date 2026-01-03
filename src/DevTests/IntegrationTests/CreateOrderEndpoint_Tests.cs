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
        var createTokenHttpRespons = await client.PostAsJsonAsync("/v1/devtokens", new CreateTokenCommand());
        var createTokenResponse = await createTokenHttpRespons.Content.ReadFromJsonAsync<CreateTokenResponse>();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", createTokenResponse?.access_token);

        var command = new CreateOrderCommand(
            CustomerId: Guid.NewGuid(),
            Lines: [new OrderLineRequest(Guid.NewGuid(), 1, 10.0m)]);

        // Act
        var response = await client.PostAsJsonAsync("/v1/orders", command);

        // Assert
        createTokenHttpRespons.EnsureSuccessStatusCode();
        createTokenHttpRespons.StatusCode.Should().Be(HttpStatusCode.Created);
    }
}
