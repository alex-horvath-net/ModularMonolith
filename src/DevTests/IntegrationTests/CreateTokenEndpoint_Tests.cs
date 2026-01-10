using System.Net.Http.Json;
using Experts.Identity.CreateToken;
using FluentAssertions;

namespace DevTests.IntegrationTests;

public class CreateTokenEndpoint_Tests(WebAppFactory factory) : IClassFixture<WebAppFactory> {

    [Fact]
    public async Task PostOrders_ShouldCreateOrder() {
        // Arrange
        var client = factory.CreateClient();
        var request = new HttpRequestMessage();
        request.Method = HttpMethod.Post;
        request.RequestUri = new Uri("/v1/devtokens", UriKind.Relative);
        request.Content = JsonContent.Create(new CreateTokenCommand(
            JwtId: Guid.NewGuid(),
            Subject: "dev-user",
            IssuedAt: DateTime.UtcNow)); 

        // act
        var respons = await client.SendAsync(request);
        
        // Assert
        respons.EnsureSuccessStatusCode();
        var token = await respons.Content.ReadFromJsonAsync<string>();
        token.Should().NotBeNull();
    }
}
