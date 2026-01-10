using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Experts.OrderExpert.PlaceOrderFlow.CreateStep;

public static class Extensions {
    public static IServiceCollection AddFactoryBusinessWorkSteps(this IServiceCollection services, IConfiguration configuration) {

        services.AddScoped<Business>();

        return services;
         
    }
}
