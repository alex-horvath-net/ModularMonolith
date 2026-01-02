using BusinessExperts.Orders.Featrures.Create;
using BusinessExperts.Orders.Featrures.Create.Infrastructure.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Testcontainers.MsSql;

namespace DevTests.IntegrationTests;

public class IntegrationTestWebAppFactory : WebApplicationFactory<global::Program>, IAsyncLifetime {

    private readonly MsSqlContainer _dbContainer = new MsSqlBuilder()
        .WithImage("mcr.microsoft.com/mssql/server:2022-latest")
        .WithPassword("Strong_password_123!")
        .Build();

    protected override void ConfigureWebHost(IWebHostBuilder builder) {
        builder.ConfigureTestServices(services => {
            var type = typeof(DbContextOptions<OrdersDbContext>);
            var descriptor = services.SingleOrDefault(s => s.ServiceType == type);
            if (descriptor is not null) {
                services.Remove(descriptor);
            }

            services.AddDbContext<OrdersDbContext>(options => options.UseSqlServer(_dbContainer.GetConnectionString()));
        });
    }

    public Task InitializeAsync() {
        return _dbContainer.StartAsync();
    }

    public new Task DisposeAsync() {
        return _dbContainer.StopAsync();
    }
}
