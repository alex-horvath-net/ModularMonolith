using Business.Events;
using Business.Modules.Billing.Infrastructure.Data;
using Business.Modules.Billing.Infrastructure.Data.Models;

namespace Business.Modules.Billing.CreateInvoice;

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