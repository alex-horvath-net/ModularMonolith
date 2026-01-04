using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using BusinessExperts.Identity.CreateToken;
using BusinessExperts.Orders.Contracts.DTOs;
using BusinessExperts.Orders.Featrures.Create;
using FluentAssertions;

namespace DevTests.IntegrationTests;

public class GetOrderByIdEndpoints_Tests(WebAppFactory factory) : IClassFixture<WebAppFactory> {

    [Fact]
    public async Task PostOrders_ShouldCreateOrder() {

        // Arrange
        var client = factory.CreateClient();
        var id = Guid.Parse("10000000-0000-0000-0000-000000000001");
        var request = new HttpRequestMessage();
        request.Method = HttpMethod.Get;
        request.RequestUri = new Uri($"/v1/orders/{id}", UriKind.Relative);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", await GetAccessToken(client));

        // Act
        var response = await client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        response.Version.ToString().Should().Be("1.1");
        response.Headers.GetValues("api-supported-versions").First().Should().Be("1.0");

        var content = await response.Content.ReadFromJsonAsync<OrderDto>();
        content?.Id.Should().Be(id);
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
