using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Routing;

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
 {
 // Placeholder for future Billing endpoints
 return app;
 }
}
