using Features.Accounts.Infrastructure.Data;
using Features.Accounts.Infrastructure.Data.Rrepositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Features.Accounts.Infrastructure;

public static class Extensions {
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration) {

        services.AddDbContext<SecurityDbContext>(options => options.UseSqlServer(configuration.GetConnectionString("AppDB"))); // scoped
        services.AddScoped<IAccountRepository, AccountRepository>();

        return services;
    }
}
