using Common;
using Orders.Contracts.Events;
using Orders.Infrastructure.Data;
using Orders.Infrastructure.Data.Models;

namespace Orders.Application.CommandHandlers;

public sealed class CreateOrderCommandHandler(OrdersDbContext db, IBusinessEventPublisher bus) {
    public async Task<Guid> Handle(CreateOrderCommand command, CancellationToken token) {
        Order order = Order.Create(command.CustomerId, command.Lines.Select(l => (l.ProductId, l.Quantity, l.UnitPrice)));
        db.Add(order);
        await db.SaveChangesAsync(token);

        decimal total = order.Lines.Sum(l => l.UnitPrice * l.Quantity);
        await bus.Publish(
            new OrderPlaced(order.Id, order.CustomerId, total),
            token);

        return order.Id;
    }
}

public sealed record CreateOrderCommand(
    Guid CustomerId,
    IEnumerable<OrderLineRequest> Lines);

public sealed record OrderLineRequest(
    Guid ProductId,
    int Quantity,
    decimal UnitPrice);
