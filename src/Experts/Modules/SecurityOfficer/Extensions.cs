using Business.Modules.SecurityOfficer.Features.Login;
using Business.Modules.SecurityOfficer.Features.Register;
using Business.Modules.SecurityOfficer.Infrastructure.Data;
using Business.Modules.SecurityOfficer.Infrastructure.GuidNumber;
using Business.Modules.SecurityOfficer.Infrastructure.Hash;
using Business.Modules.SecurityOfficer.Infrastructure.Random;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Business.Modules.SecurityOfficer;

public static class Extensions {
    public static IServiceCollection AddSecurityOfficer(this IServiceCollection services, IConfiguration configuration) {
        services.AddLogion();
        services.AddRegistration();
        services.AddDbContext<SecurityOfficerDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("AppDB")));

        // random
        services.AddSingleton<IRandom, RandomGenerator>();
        services.AddSingleton<IHasher, Pbkdf2HashGenerator>();
        services.AddSingleton<IGuid, GuidGenerator>();

        return services;
    }
}
