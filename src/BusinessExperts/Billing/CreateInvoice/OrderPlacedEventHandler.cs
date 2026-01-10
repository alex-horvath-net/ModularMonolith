using Common.Events;
using Experts.Billing.Infrastructure.Data;
using Experts.Billing.Infrastructure.Data.Models;
using Experts.Shared.Business.Events;

namespace Experts.Billing.CreateInvoice;

public sealed class OrderPlacedEventHandler(BillingDbContext db) : IBusinessEventHandler<OrderPlaced> {
    public async Task Handle(OrderPlaced orderPlaced, CancellationToken token = default) {
        var invoice = Invoice.Create(
            orderPlaced.OrderId,
            orderPlaced.CustomerId,
            orderPlaced.Total);

        db.Add(invoice);
        await db.SaveChangesAsync(token);
    }
}