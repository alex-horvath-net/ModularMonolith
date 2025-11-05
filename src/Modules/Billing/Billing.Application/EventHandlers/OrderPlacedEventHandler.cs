using Billing.Infrastructure;
using Common;
using Orders.Contracts.Events;

namespace Billing.Application.EventHandlers;

// Orders raises an event; Billing listens
public sealed class BillingOrderPlacedEventHandler(BillingDbContext db) : IBusinessEventHandler<OrderPlaced> {
    public async Task Handle(OrderPlaced e, CancellationToken token = default) {
        db.Add(Invoice.Create(e.OrderId, e.CustomerId, e.Total));
        await db.SaveChangesAsync(token);
    }
}
