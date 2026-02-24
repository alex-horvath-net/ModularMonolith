using Features.Accounts.Infrastructure;
using Features.Accounts.Slices.Login;
using Features.Accounts.Slices.Register;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Features.Accounts;

public static class Extensions {
    public static IServiceCollection AddAccounts(this IServiceCollection services, IConfiguration configuration) {
        services.AddInfrastructure(configuration);
        services.AddRegistration();
        services.AddLogion();
        return services;
    }
}
