using Experts.SecurityOfficer.Common.Infrastructure.Data;
using Experts.SecurityOfficer.Common.Infrastructure.GuidNumber;
using Experts.SecurityOfficer.Common.Infrastructure.Hash;
using Experts.SecurityOfficer.Common.Infrastructure.Random;
using Experts.SecurityOfficer.Login;
using Experts.SecurityOfficer.Register;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Experts.SecurityOfficer;

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
