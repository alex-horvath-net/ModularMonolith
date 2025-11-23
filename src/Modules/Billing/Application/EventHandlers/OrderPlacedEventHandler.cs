using Billing.Infrastructure.Data;
using Billing.Infrastructure.Data.Models;
using Common.Events;
using Orders.Contracts.Events; // Adjust when Orders contracts flattened

namespace Billing.Application.EventHandlers;

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
 