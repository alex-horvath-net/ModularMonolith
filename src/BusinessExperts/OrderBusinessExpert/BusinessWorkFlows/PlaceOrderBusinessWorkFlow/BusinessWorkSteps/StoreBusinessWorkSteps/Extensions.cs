using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BusinessExperts.OrderBusinessExpert.BusinessWorkFlows.PlaceOrderBusinessWorkFlow.BusinessWorkSteps.StoreBusinessWorkSteps;

public static class Extensions {
    public static IServiceCollection AddStoreBusinessWorkSteps(this IServiceCollection services, IConfiguration configuration) {

        services.AddScoped<Business>();
        services.AddScoped<Business.IInfrastructure, Infrastructure>();

        return services;

    }

}
