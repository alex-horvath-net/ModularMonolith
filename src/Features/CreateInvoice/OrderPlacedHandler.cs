using Billing.Core.Infrastructure.Data;
using Billing.Core.Infrastructure.Data.Models;
using Core.Events;

namespace Billing.CreateInvoice;

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