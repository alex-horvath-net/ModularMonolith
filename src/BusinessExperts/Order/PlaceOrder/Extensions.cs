using Experts.Order.PlaceOrder.FactoryBusinessWorkSteps;
using Experts.Order.PlaceOrder.PublisherBusinessWorkStep;
using Experts.Order.PlaceOrder.StoreBusinessWorkSteps;
using Experts.Order.PlaceOrder.ValidatorBusinessWorkSteps;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Experts.Order.PlaceOrder;

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
