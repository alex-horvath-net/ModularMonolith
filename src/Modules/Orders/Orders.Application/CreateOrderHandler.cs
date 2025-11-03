using Common;
using Orders.Contracts;
using Orders.Infrastructure;

namespace Orders.Application;


public sealed class CreateOrderHandler(OrdersDbContext db, IEventBus bus) {
    public async Task<Guid> Handle(CreateOrderRequest req) {
        var order = Order.Create(req.CustomerId, req.Lines.Select(l => (l.ProductId, l.Quantity, l.UnitPrice)));
        db.Add(order);
        await db.SaveChangesAsync();

        var total = order.Lines.Sum(l => l.UnitPrice * l.Quantity);
        await bus.Publish(new OrderPlaced(order.Id, order.CustomerId, total));
        return order.Id;
    }
}


public sealed record CreateOrderRequest(
    Guid CustomerId,
    IEnumerable<OrderLineRequest> Lines);

public sealed record OrderLineRequest(
    Guid ProductId,
    int Quantity,
    decimal UnitPrice);
