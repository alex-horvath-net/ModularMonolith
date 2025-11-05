using Billing.Application.EventHandlers;
using Common;
using Microsoft.Extensions.DependencyInjection;
using Orders.Contracts.Events;

namespace Billing.Application; 
public static class ApplicationExtensions {
    public static void AddBillingApplication(this IServiceCollection services) =>
        services.AddScoped<IBusinessEventHandler<OrderPlaced>, BillingOrderPlacedEventHandler>();
}
    