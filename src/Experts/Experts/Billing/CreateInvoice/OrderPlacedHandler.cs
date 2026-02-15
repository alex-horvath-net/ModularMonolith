using Business.Events;
using Business.Experts.Billing.Infrastructure.Data;
using Business.Experts.Billing.Infrastructure.Data.Models;

namespace Business.Experts.Billing.CreateInvoice;

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