using Accounts.Core.Infrastructure;
using Accounts.Login;
using Accounts.Register;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Accounts;

public static class Extensions {
    public static IServiceCollection AddAccounts(this IServiceCollection services, IConfiguration configuration) {
        services.AddInfrastructure(configuration);
        services.AddRegistration();
        services.AddLogion();
        return services;
    }
}
