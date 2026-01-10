using BusinessExperts.BillingBusinessExpert.Infrastructure.Data;
using BusinessExperts.BillingBusinessExpert.Infrastructure.Data.Models;
using BusinessExperts.Shared.Business.Events;
using Common.Events;

namespace BusinessExperts.BillingBusinessExpert.CreateInvoice;

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