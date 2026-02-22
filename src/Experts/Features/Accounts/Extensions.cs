using Business.Features.Accounts.Infrastructure;
using Business.Features.Accounts.Infrastructure.Data;
using Business.Features.Accounts.Infrastructure.Data.Rrepositories;
using Business.Features.Accounts.Slices.Login;
using Business.Features.Accounts.Slices.Register;
using Business.Infrastructure;
using Business.Infrastructure.Clock;
using Business.Infrastructure.GuidNumber;
using Business.Infrastructure.Hash;
using Business.Infrastructure.Random;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Business.Features.Accounts;

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
