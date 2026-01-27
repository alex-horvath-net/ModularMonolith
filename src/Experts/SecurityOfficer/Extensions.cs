using Experts.SecurityOfficer.Login;
using Experts.SecurityOfficer.Shared.Infrastructure.Data;
using Microsoft.Extensions.DependencyInjection;

namespace Experts.SecurityOfficer; 
public static class Extensions {
    public static IServiceCollection AddSecurityOfficer(this IServiceCollection services) {
        services.AddLogion();
        services.AddScoped<SecurityOfficerDbContext>();
        return services;
    }
}
