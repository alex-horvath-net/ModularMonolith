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

        var accessToken = await GetAccessToken(client);

        var command = new CreateOrderCommand(
            CustomerId: Guid.NewGuid(),
            Lines: [new OrderLineRequest(Guid.NewGuid(), 1, 10.0m)]);

        var request = new HttpRequestMessage {
            Method = HttpMethod.Post,
            RequestUri = new Uri("/v1/orders", UriKind.Relative),
            Content = JsonContent.Create(command)
        };
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
       
        var response = await client.SendAsync(request);

        // Assert
        response.EnsureSuccessStatusCode();
        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    private static async Task<string> GetAccessToken(HttpClient client) {
        var request = new HttpRequestMessage {
            Method = HttpMethod.Post,
            RequestUri = new Uri("/v1/devtokens", UriKind.Relative),
            Content = JsonContent.Create(new CreateTokenCommand())
        };
        
        var response = await client.SendAsync(request);
        response.EnsureSuccessStatusCode();
        var token = await response.Content.ReadFromJsonAsync<string>();
        token.Should().NotBeNullOrWhiteSpace();
        return token!;
    }
}
