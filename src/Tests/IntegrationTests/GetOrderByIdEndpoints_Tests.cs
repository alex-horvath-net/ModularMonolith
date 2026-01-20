using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;

namespace Tests.IntegrationTests;

public class GetOrderByIdEndpoints_Tests(WebAppFactory factory) : IClassFixture<WebAppFactory> {

    [Fact]
    public async Task GetOrderById_Should_rRtutn_Order() {

        // Arrange
        var orderId = "EF955A1E-6AED-4B93-AA3C-05090CED8187";

        // Act
        var response = await factory.Get($"/v1/orders/{orderId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        response.Version.ToString().Should().Be("1.1");
        response.Headers.GetValues("api-supported-versions").First().Should().Be("1.0");

        var content = await response.Content.ReadFromJsonAsync<JsonElement>();
        content.GetProperty("id").GetGuid().Should().Be(orderId);
    }
}
