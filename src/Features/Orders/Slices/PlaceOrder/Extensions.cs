using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Features.Orders.Slices.PlaceOrder;

public static class Extensions {
    public static IServiceCollection AddPlaceOrderBusinessWorkFlow(this IServiceCollection services) {

        services.AddScoped<BusinessWorkFlow>();
        services.AddScoped<BusinessWorkFlow.IBusinessWorkSteps, BusinessWorkSteps>();
        services.AddScoped<IValidator<PlaceOrderRequest>, Infrastructure.Validator>();
        services.AddScoped<BusinessWorkSteps.IStoreInfrastructure, Infrastructure.Store>();

        return services;

    }

}
