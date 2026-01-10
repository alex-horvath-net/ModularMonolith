using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Experts.OrderBusinessExpert.BusinessWorkFlows.PlaceOrderBusinessWorkFlow.BusinessWorkSteps.PublisherBusinessWorkStep;

public static class Extensions {
    public static IServiceCollection AddPublisherBusinessWorkStep(this IServiceCollection services, IConfiguration configuration) {

        services.AddScoped<Business>();

        return services;

    }

}
