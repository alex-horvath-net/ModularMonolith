using Experts.SecurityOfficer.Login;
using Experts.SecurityOfficer.Register;
using Experts.SecurityOfficer.Shared.Infrastructure.Data;
using Experts.SecurityOfficer.Shared.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Experts.SecurityOfficer; 
public static class Extensions {
    public static IServiceCollection AddSecurityOfficer(this IServiceCollection services) {
        services.AddLogion();
        services.AddRegistration();
        services.AddDbContext<SecurityOfficerDbContext>(options =>
            options.UseInMemoryDatabase("SecurityOfficer_Identity"));
        services.TryAddSingleton<IPasswordHasher, Pbkdf2PasswordHasher>();
        return services;
    }
}
