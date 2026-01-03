using BusinessExperts.Billing;
using BusinessExperts.Orders;
using Common;
using Common.Authentication;
using Docker.DotNet.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Org.BouncyCastle.Tls;

namespace DevTests.IntegrationTests;

public abstract class BaseIntegrationTest : IClassFixture<IntegrationTestWebAppFactory>, IDisposable {
    private readonly IServiceScope _scope = default!;
    protected readonly HttpClient TestClient = default!;
    protected BaseIntegrationTest(IntegrationTestWebAppFactory factory) {
        _scope = factory.Services.CreateScope();

        TestClient = factory.CreateClient();
    }
    public T Get<T>() where T : notnull => _scope.ServiceProvider.GetRequiredService<T>();
    public void Dispose() {
        _scope?.Dispose();
       
    }
}

