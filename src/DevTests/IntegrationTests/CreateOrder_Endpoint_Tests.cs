using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using BusinessExperts.IdentityBusinessExpert.CreateToken;
using BusinessExperts.OrderBusinessExpert.WorkFlows.PlaceOrderBusinessWorkFlow.Shared.Business.Domain;
using FluentAssertions;

namespace DevTests.IntegrationTests;

public class CreateOrder_Endpoint_Tests(WebAppFactory factory) : IClassFixture<WebAppFactory> {

    [Fact]
    public async Task PostOrders_ShouldCreateOrder() {

        // Arrange
        var client = factory.CreateClient();

        var request = new HttpRequestMessage();
        request.Method = HttpMethod.Post;
        request.RequestUri = new Uri("/v1/orders", UriKind.Relative);
        request.Content = JsonContent.Create(GetCreateOrderRequest());
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", await GetAccessToken(client));

        // Act
        var response = await client.SendAsync(request);

        // Assert
        response.EnsureSuccessStatusCode();
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        response.Version.ToString().Should().Be("1.1");
        response.Headers.GetValues("api-supported-versions").First().Should().Be("1.0");
        response.Headers.Location?.OriginalString.Should().StartWith("/v1/orders/");

        var content = await response.Content.ReadFromJsonAsync<Guid>();
        content.Should().NotBe(Guid.Empty);
    }

    private static CreateOrderRequest GetCreateOrderRequest() {
        return new CreateOrderRequest(
                    CustomerId: Guid.NewGuid(),
                    Lines: [new CreateOrderLineRequest(Guid.NewGuid(), 1, 100.0m)]);
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
