using Experts.OrderBusinessExpert.PlaceOrderBusinessWorkFlow.FactoryBusinessWorkSteps;
using Experts.OrderBusinessExpert.PlaceOrderBusinessWorkFlow.PublisherBusinessWorkStep;
using Experts.OrderBusinessExpert.PlaceOrderBusinessWorkFlow.StoreBusinessWorkSteps;
using Experts.OrderBusinessExpert.PlaceOrderBusinessWorkFlow.ValidatorBusinessWorkSteps;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Experts.OrderBusinessExpert.PlaceOrderBusinessWorkFlow;

public static class Extensions {
    public static IServiceCollection AddPlaceOrderBusinessWorkFlow(this IServiceCollection services, IConfiguration configuration) {

        services.AddScoped<PlaceOrderWorkflow>();

        services.AddValidatorBusinessWorkSteps(configuration);
        services.AddFactoryBusinessWorkSteps(configuration);
        services.AddStoreBusinessWorkSteps(configuration);
        services.AddPublisherBusinessWorkStep(configuration);

        return services;

    }

}
