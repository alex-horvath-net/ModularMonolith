using Business.Experts.SecurityOfficer.Infrastructure;
using Business.Experts.SecurityOfficer.Infrastructure.Data;
using Business.Experts.SecurityOfficer.Infrastructure.GuidNumber;
using Business.Experts.SecurityOfficer.Infrastructure.Hash;
using Business.Experts.SecurityOfficer.Infrastructure.Random;
using Business.Experts.SecurityOfficer.UserStories.Login;
using Business.Experts.SecurityOfficer.UserStories.Register;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Business.Experts.SecurityOfficer;

public static class Extensions {
    public static IServiceCollection AddSecurityOfficer(this IServiceCollection services, IConfiguration configuration) {
        services.AddLogion();
        services.AddRegistration();
        services.AddDbContext<SecurityDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("AppDB")));

        // random
        services.AddSingleton<IRandom, RandomGenerator>();
        services.AddSingleton<IHasher, Pbkdf2HashGenerator>();
        services.AddSingleton<IGuid, GuidGenerator>();

        return services;
    }
}
