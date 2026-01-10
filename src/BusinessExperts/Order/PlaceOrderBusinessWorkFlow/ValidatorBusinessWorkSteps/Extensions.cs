using Experts.Order.PlaceOrderBusinessWorkFlow.Shared.Business.Domain;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Experts.Order.PlaceOrderBusinessWorkFlow.ValidatorBusinessWorkSteps;

public static class Extensions {
    public static IServiceCollection AddValidatorBusinessWorkSteps(this IServiceCollection services, IConfiguration configuration) {

        services.AddScoped<Business>();
        services.AddScoped<IValidator<CreateOrderRequest>, Infrastructure>();

        return services;

    }

}
