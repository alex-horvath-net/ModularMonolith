using Core.Infrastructure;
using Core.Infrastructure.Clock;
using Core.Infrastructure.GuidNumber;
using Core.Infrastructure.Hash;
using Core.Infrastructure.Random;
using Features.Accounts.Infrastructure;
using Features.Accounts.Infrastructure.Data;
using Features.Accounts.Infrastructure.Data.Rrepositories;
using Features.Accounts.Slices.Login;
using Features.Accounts.Slices.Register;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Features.Accounts;

public static class Extensions {
    public static IServiceCollection AddAccounts(this IServiceCollection services, IConfiguration configuration) {
        services.AddRegistration();
        services.AddLogion();

        services.AddDbContext<SecurityDbContext>(options => options.UseSqlServer(configuration.GetConnectionString("AppDB"))); // scoped
        services.AddSingleton<IAccountRepository, AccountRepository>();
        services.AddSingleton<IRandom, RandomGenerator>();
        services.AddSingleton<IHasher, Pbkdf2HashGenerator>();
        services.AddSingleton<IGuid, GuidGenerator>();
        services.AddSingleton<IClock, SystemClock>();

        return services;
    }
}
