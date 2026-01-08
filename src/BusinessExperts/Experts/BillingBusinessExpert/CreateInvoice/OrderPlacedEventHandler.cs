using Common.Events;
using Experts.Experts.BillingBusinessExpert.Infrastructure.Data;
using Experts.Experts.BillingBusinessExpert.Infrastructure.Data.Models;
using Experts.Experts.Shared.Business.Events;

namespace Experts.Experts.BillingBusinessExpert.CreateInvoice;

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