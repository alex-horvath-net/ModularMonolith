using Experts.Order.PlaceOrderBusinessWorkFlow.FactoryBusinessWorkSteps;
using Experts.Order.PlaceOrderBusinessWorkFlow.PublisherBusinessWorkStep;
using Experts.Order.PlaceOrderBusinessWorkFlow.StoreBusinessWorkSteps;
using Experts.Order.PlaceOrderBusinessWorkFlow.ValidatorBusinessWorkSteps;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Experts.Order.PlaceOrderBusinessWorkFlow;

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
