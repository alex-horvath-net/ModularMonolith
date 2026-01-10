using Experts.OrderExpert.PlaceOrderFlow.CreateStep;
using Experts.OrderExpert.PlaceOrderFlow.PublishStep;
using Experts.OrderExpert.PlaceOrderFlow.SaveStep;
using Experts.OrderExpert.PlaceOrderFlow.ValideStep;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Experts.OrderExpert.PlaceOrderFlow;

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
