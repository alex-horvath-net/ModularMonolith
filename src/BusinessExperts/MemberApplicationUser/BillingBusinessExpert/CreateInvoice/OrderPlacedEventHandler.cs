using Business.MemberApplicationUser.BillingBusinessExpert.Infrastructure.Data;
using Business.MemberApplicationUser.BillingBusinessExpert.Infrastructure.Data.Models;
using Business.MemberApplicationUser.Contracts.Events;
using Common.Events;

namespace Business.MemberApplicationUser.BillingBusinessExpert.CreateInvoice;

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