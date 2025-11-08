using Common;
using Microsoft.Extensions.DependencyInjection;
using Orders.Contracts.Events; // Adjust when Orders contracts flattened

namespace Billing;

public static class BillingApplicationExtensions
{
 public static IServiceCollection AddBillingApplication(this IServiceCollection services)
 {
 services.AddScoped<IBusinessEventHandler<OrderPlaced>, BillingOrderPlacedEventHandler>();
 return services;
 }
}

public sealed class BillingOrderPlacedEventHandler(BillingDbContext db) : IBusinessEventHandler<OrderPlaced>
{
 public async Task Handle(OrderPlaced e, CancellationToken token = default)
 {
 db.Add(Invoice.Create(e.OrderId, e.CustomerId, e.Total));
 await db.SaveChangesAsync(token);
 }
}
