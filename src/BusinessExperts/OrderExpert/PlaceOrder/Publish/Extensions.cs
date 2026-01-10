using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Experts.OrderExpert.PlaceOrder.Publish;

public static class Extensions {
    public static IServiceCollection AddPublisherBusinessWorkStep(this IServiceCollection services, IConfiguration configuration) {

        services.AddScoped<Business>();

        return services;

    }

}
