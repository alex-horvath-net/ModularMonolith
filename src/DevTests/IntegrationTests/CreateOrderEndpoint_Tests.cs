using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using BusinessExperts.Identity.CreateToken;
using BusinessExperts.Orders.Featrures.Create;
using FluentAssertions;

namespace DevTests.IntegrationTests;

public class CreateOrderEndpoint_Tests(WebAppFactory factory) : IClassFixture<WebAppFactory> {

    [Fact]
    public async Task PostOrders_ShouldCreateOrder() {

        // Arrange
        var client = factory.CreateClient();

        var request = new HttpRequestMessage();
        request.Method = HttpMethod.Post;
        request.RequestUri = new Uri("/v1/orders", UriKind.Relative);
        request.Content = JsonContent.Create(GetCreateOrderCommand());
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", await GetAccessToken(client));

        var response = await client.SendAsync(request);

        // Assert
        response.EnsureSuccessStatusCode();
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var content = await response.Content.ReadFromJsonAsync<JsonElement>();
        content.GetProperty("id").GetGuid().Should().NotBe(Guid.Empty);
    }

    private static CreateOrderCommand GetCreateOrderCommand() {
        return new CreateOrderCommand(
                    CustomerId: Guid.NewGuid(),
                    Lines: [new OrderLineRequest(Guid.NewGuid(), 1, 10.0m)]);
    }

    private static async Task<string> GetAccessToken(HttpClient client) {
        var request = new HttpRequestMessage();
        request.Method = HttpMethod.Post;
        request.RequestUri = new Uri("/v1/devtokens", UriKind.Relative);
        request.Content = JsonContent.Create(new CreateTokenCommand(
            JwtId: Guid.NewGuid(),
            Subject: "dev-user",
            IssuedAt: DateTime.UtcNow));

        var response = await client.SendAsync(request);
        response.EnsureSuccessStatusCode();
        var token = await response.Content.ReadFromJsonAsync<string>();
        token.Should().NotBeNullOrWhiteSpace();
        return token!;
    }
}
