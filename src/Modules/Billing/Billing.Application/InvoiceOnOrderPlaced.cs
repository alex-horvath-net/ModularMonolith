using Billing.Infrastructure;
using Common;
using Orders.Contracts;

namespace Billing.Application;

public sealed class InvoiceOnOrderPlaced(BillingDbContext db) : IEventHandler<OrderPlaced> {
    public async Task Handle(OrderPlaced e, CancellationToken ct = default) {
        db.Add(Invoice.Create(e.OrderId, e.CustomerId, e.Total));
        await db.SaveChangesAsync(ct);
    }
}
