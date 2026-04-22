using Accounts.Core.Infrastructure.Data;
using Accounts.Core.Infrastructure.Data.Rrepositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Accounts.Core.Infrastructure;

public static class Extensions {
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration) {

        services.AddDbContext<AccountDbContext>(options => {
            var connectionString = configuration.GetConnectionString("AppDB");
            options.UseSqlServer(connectionString);
        }); // scoped

        services.AddScoped<IAccountRepository, AccountRepository>();

        return services;
    }
}
