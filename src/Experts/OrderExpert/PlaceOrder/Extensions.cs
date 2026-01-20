using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Experts.OrderExpert.PlaceOrder;

public static class Extensions {
    public static IServiceCollection AddPlaceOrderBusinessWorkFlow(this IServiceCollection services, IConfiguration configuration) {

        services.AddScoped<BusinessWorkFlow>();
        services.AddScoped<BusinessWorkFlow.IBusinessWorkSteps, BusinessWorkSteps>();
        services.AddScoped<IValidator<PlaceOrderRequest>, Infrastructure.Validator>();
        services.AddScoped<BusinessWorkSteps.IStoreInfrastructure, Infrastructure.Store>();

        return services;

    }

}
