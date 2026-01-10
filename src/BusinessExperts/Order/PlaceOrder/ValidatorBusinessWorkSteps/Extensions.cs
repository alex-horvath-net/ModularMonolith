using Experts.Order.PlaceOrder.Shared.Business.Domain;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Experts.Order.PlaceOrder.ValidatorBusinessWorkSteps;

public static class Extensions {
    public static IServiceCollection AddValidatorBusinessWorkSteps(this IServiceCollection services, IConfiguration configuration) {

        services.AddScoped<Business>();
        services.AddScoped<IValidator<CreateOrderRequest>, Infrastructure>();

        return services;

    }

}
