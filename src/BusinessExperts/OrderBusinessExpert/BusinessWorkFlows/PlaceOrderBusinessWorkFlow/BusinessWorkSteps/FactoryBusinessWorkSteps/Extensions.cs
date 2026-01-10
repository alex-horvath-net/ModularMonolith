using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Experts.OrderBusinessExpert.BusinessWorkFlows.PlaceOrderBusinessWorkFlow.BusinessWorkSteps.FactoryBusinessWorkSteps;

public static class Extensions {
    public static IServiceCollection AddFactoryBusinessWorkSteps(this IServiceCollection services, IConfiguration configuration) {

        services.AddScoped<Business>();

        return services;
         
    }
}
