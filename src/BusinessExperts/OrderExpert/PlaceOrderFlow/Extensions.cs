using Experts.OrderExpert.PlaceOrderFlow.Shared.Business.Domain;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Experts.OrderExpert.PlaceOrderFlow;

public static class Extensions {
    public static IServiceCollection AddPlaceOrderBusinessWorkFlow(this IServiceCollection services, IConfiguration configuration) {

        services.AddScoped<BusinessWorkFlow>();
        services.AddScoped<BusinessWorkFlow.IBusinessWorkSteps, BusinessWorkSteps>();
        services.AddScoped<IValidator<CreateOrderRequest>, ValidatorInfrastructure>();
        services.AddScoped<BusinessWorkSteps.IStoreInfrastructure, StoreInfrastructure>();

        return services;

    }

}
