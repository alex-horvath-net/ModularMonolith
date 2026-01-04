using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Business.ApplicationUsers.Member.Orders.Contracts.DTOs;
using BusinessExperts.Identity.CreateToken;
using BusinessExperts.Orders.Featrures.Create;
using FluentAssertions;

namespace DevTests.IntegrationTests;

public class GetAllOrderEndpoints_Tests(WebAppFactory factory) : IClassFixture<WebAppFactory> {

    [Fact]
    public async Task PostOrders_ShouldCreateOrder() {

        // Arrange
        var client = factory.CreateClient();

        var request = new HttpRequestMessage();
        request.Method = HttpMethod.Get;
        request.RequestUri = new Uri("/v1/orders", UriKind.Relative);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", await GetAccessToken(client));

        // Act
        var response = await client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        response.Version.ToString().Should().Be("1.1");
        response.Headers.GetValues("api-supported-versions").First().Should().Be("1.0");

        var content = await response.Content.ReadFromJsonAsync<List<OrderDto>>();
        content.Should().NotBeEmpty();
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
