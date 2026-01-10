using Experts.OrderExpert.PlaceOrderFlow.Shared.Business.Domain;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Experts.OrderExpert.PlaceOrderFlow.ValideStep;

public static class Extensions {
    public static IServiceCollection AddValidatorBusinessWorkSteps(this IServiceCollection services, IConfiguration configuration) {

        services.AddScoped<Business>();
        services.AddScoped<Business.IAdapter, Adapter>();
        services.AddScoped<IValidator<CreateOrderRequest>, Infrastructure>();

        return services;

    }

}
