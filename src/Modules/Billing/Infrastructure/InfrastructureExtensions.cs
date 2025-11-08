using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Billing;

public static class BillingInfrastructureExtensions
{
 public static IServiceCollection AddBillingInfrastructure(this IServiceCollection services, IConfiguration configuration)
 {
 services.AddDbContext<BillingDbContext>(opts =>
 opts.UseSqlServer(configuration.GetConnectionString("Default")));
 return services;
 }
}
