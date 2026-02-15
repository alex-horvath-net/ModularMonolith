using System.Net.Http.Headers;
using System.Net.Http.Json;
using Business.Experts.Billing.Infrastructure.Data;
using Business.Experts.OrderExpert.Common.Infrastructure.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Tests.IntegrationTests;

public class WebAppFactory : WebApplicationFactory<TradingApi.User>, IAsyncLifetime {


    private async Task<string?> GetAccessToken(HttpClient client) {
        var content = new CreateTokenCommand(
            JwtId: Guid.NewGuid(),
            Subject: "dev-user",
            IssuedAt: DateTime.UtcNow);


        var request = new HttpRequestMessage(HttpMethod.Post, "/v1/devtokens") {
            Content = JsonContent.Create(content)
        };
        var response = await client.SendAsync(request);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<string>();
    }


    public async Task<HttpResponseMessage> Get(string url, Action<HttpRequestMessage>? config = null) {

        var client = CreateClient();

        var request = new HttpRequestMessage(HttpMethod.Get, url);
        if (config is not null) {
            config(request);
        }
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", await GetAccessToken(client));

        return await client.SendAsync(request);
    }


    public async Task<HttpResponseMessage> Post<T>(string url, T content, Action<HttpRequestMessage>? config = null) {

        var client = CreateClient();

        var request = new HttpRequestMessage(HttpMethod.Post, url);
        if (config is not null) {
            config(request);
        }
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", await GetAccessToken(client));
        request.Content = JsonContent.Create(content);
        return await client.SendAsync(request);
    }


    private IServiceScope scope = default!;

    public T GetRequiredService<T>() where T : notnull => scope.ServiceProvider.GetRequiredService<T>();


    protected override void ConfigureWebHost(IWebHostBuilder builder) {
        builder.UseSetting(WebHostDefaults.EnvironmentKey, "IntegrationTest");
        builder.UseTestServer();
        builder.ConfigureAppConfiguration((_, configBuilder) => {
            configBuilder.AddInMemoryCollection(new Dictionary<string, string?> {
                ["ConnectionStrings:AppDB"] = "Server=.\\SQLEXPRESS;Database=AppDb_Test;User ID=sa;Password=Dev!Pass123;TrustServerCertificate=True;Encrypt=False",
                ["Authentication:Issuer"] = "integration-tests",
                ["Authentication:Audience"] = "integration-tests",
                ["Authentication:SecurityKey"] = "integration-test-key-012345678901234567890123456789",
                ["Authentication:AllowDevSymmetricKey"] = "true",
                ["Authentication:DevScopes:0"] = "orders.read",
                ["Authentication:DevScopes:1"] = "orders.write",
                ["Authentication:DevScopes:2"] = "billing.read"
            });
        });
    }

    public async Task InitializeAsync() {
        scope = Services.CreateScope();

        var ordersDb = GetRequiredService<OrdersDbContext>();
        await ordersDb.Database.EnsureCreatedAsync();

        var billingDb = GetRequiredService<BillingDbContext>();
        await billingDb.Database.EnsureCreatedAsync();
    }

    public new Task DisposeAsync() {
        scope?.Dispose();
        return Task.CompletedTask;
    }
}

