using Business.Experts.BillingBusinessExpert.Infrastructure.Data;
using Business.Experts.BillingBusinessExpert.Infrastructure.Data.Models;
using Business.Experts.Shared.Business.Events;
using Common.Events;

namespace Business.Experts.BillingBusinessExpert.CreateInvoice;

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