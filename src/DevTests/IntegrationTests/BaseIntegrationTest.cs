using System.Net.Http.Headers;
using BusinessExperts.Billing;
using BusinessExperts.Identity.CreateToken;
using BusinessExperts.Orders;
using BusinessExperts.Orders.Featrures.Create.Infrastructure.Data;
using Common;
using Common.Authentication;
using Docker.DotNet.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Org.BouncyCastle.Tls;
using Testcontainers.MsSql;

namespace DevTests.IntegrationTests;

public class IntegrationTestWebAppFactory : WebApplicationFactory<Program>, IAsyncLifetime {

    private readonly MsSqlContainer _dbContainer = new MsSqlBuilder("mcr.microsoft.com/mssql/server:2022-latest")
        .WithPassword("Strong_password_123!")
        .Build();
    private string token = default!;

     override protected void ConfigureWebHost(IWebHostBuilder builder) {
        builder.ConfigureTestServices(services => {
            UseTestContainerDB<OrdersDbContext>(services);
        });
    }


    override protected void ConfigureClient(HttpClient client) {
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }

  

    public async Task InitializeAsync() {
        var scope = Services.CreateScope();

        await _dbContainer.StartAsync();


        using var ordersDb = scope.ServiceProvider.GetRequiredService<OrdersDbContext>();
        ordersDb.Database.Migrate();



        var tokenHandler = scope.ServiceProvider.GetRequiredService<CreateTokenCommandHandler>();
        token = await tokenHandler.Handle(new CreateTokenCommand());
    }

    public new Task DisposeAsync() {
        return _dbContainer.StopAsync();
    }

    private void UseTestContainerDB<T>(IServiceCollection services) where T : DbContext {
        var descriptorType = typeof(DbContextOptions<T>);
        var descriptor = services.SingleOrDefault(s => s.ServiceType == descriptorType);
        if (descriptor is not null) {
            services.Remove(descriptor);
        }

        services.AddDbContext<T>(options => options.UseSqlServer(_dbContainer.GetConnectionString()));
    }
}

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

