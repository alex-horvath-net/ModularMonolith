using Billing.Infrastructure.Data;
using Billing.Infrastructure.Data.Models;
using Core.Events;

namespace Billing.Slices.CreateInvoice;

public sealed class OrderPlacedHandler(BillingDbContext db) : IBusinessHandler<OrderPlaced> {
    public async Task Handle(OrderPlaced orderPlaced, CancellationToken token = default) {
        var invoice = Invoice.Create(
            orderPlaced.OrderId,
            orderPlaced.CustomerId,
            orderPlaced.Total);

        db.Add(invoice);
        await db.SaveChangesAsync(token);
    }
}