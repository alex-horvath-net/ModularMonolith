using Business.ApplicationUsers.Member.Billing.Infrastructure.Data;
using Business.ApplicationUsers.Member.Billing.Infrastructure.Data.Models;
using Business.ApplicationUsers.Member.Contracts.Events;
using Common.Events;

namespace Business.ApplicationUsers.Member.Billing.CreateInvoice;

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