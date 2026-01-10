using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Experts.IdentityBusinessExpert.CreateToken;
using FluentAssertions;
using Experts.OrderBusinessExpert.BusinessWorkFlows.PlaceOrderBusinessWorkFlow.BusinessWorkSteps.Shared.Business.Domain;
using Experts.OrderBusinessExpert.BusinessWorkFlows.PlaceOrderBusinessWorkFlow.BusinessWorkSteps.Shared.Business.Domain;

namespace DevTests.IntegrationTests;

public class GetOrderByIdEndpoints_Tests(WebAppFactory factory) : IClassFixture<WebAppFactory> {

    [Fact]
    public async Task PostOrders_ShouldCreateOrder() {

        // Arrange
        var client = factory.CreateClient();
        var token = await GetAccessToken(client);
        var createdId = await CreateOrderAsync(client, token);

        var request = new HttpRequestMessage();
        request.Method = HttpMethod.Get;
        request.RequestUri = new Uri($"/v1/orders/{createdId}", UriKind.Relative);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        response.Version.ToString().Should().Be("1.1");
        response.Headers.GetValues("api-supported-versions").First().Should().Be("1.0");

        var content = await response.Content.ReadFromJsonAsync<JsonElement>();
        content.GetProperty("id").GetGuid().Should().Be(createdId);
    }

    private static async Task<Guid> CreateOrderAsync(HttpClient client, string token) {
        var request = new HttpRequestMessage();
        request.Method = HttpMethod.Post;
        request.RequestUri = new Uri("/v1/orders", UriKind.Relative);
        request.Content = JsonContent.Create(GetCreateOrderRequest());
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await client.SendAsync(request);
        response.EnsureSuccessStatusCode();
        var id = await response.Content.ReadFromJsonAsync<Guid>();
        id.Should().NotBe(Guid.Empty);
        return id;
    }

    private static CreateOrderRequest GetCreateOrderRequest() {
        return new CreateOrderRequest(
                    CustomerId: Guid.NewGuid(),
                    Lines: [new CreateOrderLineRequest(Guid.NewGuid(), 1, 10.0m)]);
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
