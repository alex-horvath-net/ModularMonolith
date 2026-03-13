using Accounts.Infrastructure.Data;
using Accounts.Infrastructure.Data.Rrepositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Accounts.Infrastructure;

public static class Extensions {
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration) {

        services.AddDbContext<SecurityDbContext>(options => {
            var connectionString = configuration.GetConnectionString("AppDB");
            options.UseSqlServer(connectionString);
        }); // scoped

        services.AddScoped<IAccountRepository, AccountRepository>();

        return services;
    }
}
