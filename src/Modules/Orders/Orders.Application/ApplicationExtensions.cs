using Microsoft.Extensions.DependencyInjection;
using Orders.Application.CommandHandlers;
using Orders.Application.QueryHandlers;
using Orders.Application.QueryServices;
using Orders.Contracts.Services;

namespace Orders.Application;
public static class ApplicationExtensions {
    public static IServiceCollection AddOrdersApplication(this IServiceCollection services) => services
        .AddScoped<CreateOrderCommandHandler>()
        .AddScoped<GetOrderQueryHandler>()
        .AddScoped<IReadOrderService, ReadOrderService>();
}
