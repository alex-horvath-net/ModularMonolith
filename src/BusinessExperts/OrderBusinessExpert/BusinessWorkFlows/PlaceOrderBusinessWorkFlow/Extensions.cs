using Experts.OrderBusinessExpert.BusinessWorkFlows.PlaceOrderBusinessWorkFlow.BusinessWorkSteps.FactoryBusinessWorkSteps;
using Experts.OrderBusinessExpert.BusinessWorkFlows.PlaceOrderBusinessWorkFlow.BusinessWorkSteps.PublisherBusinessWorkStep;
using Experts.OrderBusinessExpert.BusinessWorkFlows.PlaceOrderBusinessWorkFlow.BusinessWorkSteps.StoreBusinessWorkSteps;
using Experts.OrderBusinessExpert.BusinessWorkFlows.PlaceOrderBusinessWorkFlow.BusinessWorkSteps.ValidatorBusinessWorkSteps;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Experts.OrderBusinessExpert.BusinessWorkFlows.PlaceOrderBusinessWorkFlow;

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
