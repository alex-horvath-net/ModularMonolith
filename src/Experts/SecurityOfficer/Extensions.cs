using Experts.SecurityOfficer.Common.Infrastructure.Cryptography;
using Experts.SecurityOfficer.Common.Infrastructure.Data;
using Experts.SecurityOfficer.Login;
using Experts.SecurityOfficer.Register;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Experts.SecurityOfficer;

public static class Extensions {
    public static IServiceCollection AddSecurityOfficer(this IServiceCollection services, IConfiguration configuration) {
        services.AddLogion();
        services.AddRegistration();
        services.AddDbContext<SecurityOfficerDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("AppDB")));
        services.TryAddSingleton<IPasswordHasher, Pbkdf2PasswordHasher>();
        return services;
    }
}
