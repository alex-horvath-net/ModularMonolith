using BusinessExperts.Billing.Infrastructure.Data;
using BusinessExperts.Billing.Infrastructure.Data.Models;
using BusinessExperts.Orders.Contracts.Events;
using Common.Events;

namespace BusinessExperts.Billing.CreateInvoice;

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