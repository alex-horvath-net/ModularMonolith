using System.Net;
using System.Net.Http.Json;
using BusinessExperts.Identity.CreateToken;
using FluentAssertions;

namespace DevTests.IntegrationTests;

public class CreateTokenEndpoint_Tests(WebAppFactory factory) : IClassFixture<WebAppFactory> {

    [Fact]
    public async Task PostOrders_ShouldCreateOrder() {

        // Arrange
        var client = factory.CreateClient();
        var command = new CreateTokenCommand();

        // Act
        var response = await client.PostAsJsonAsync("/v1/devtokens", command);

        // Assert
        response.EnsureSuccessStatusCode();
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var createdToken = await response.Content.ReadFromJsonAsync<CreateTokenResponse>();
        createdToken.Should().NotBeNull();
        createdToken!.access_token.Should().NotBeNullOrEmpty();
    }
}
