using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Routing;
using Billing.API; // endpoint mapping

namespace Billing;

public static class BillingModuleExtensions
{
 public static IServiceCollection AddBilling(this IServiceCollection services, IConfiguration configuration)
 {
 services.AddBillingInfrastructure(configuration)
 .AddBillingApplication();
 return services;
 }

 public static IEndpointRouteBuilder MapBilling(this IEndpointRouteBuilder app)
 => BillingEndpoints.MapBilling(app);
}
