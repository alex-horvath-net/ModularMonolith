using Orders.Infrastructure;
using Orders.Contracts.DTOs;

namespace Orders.Application;

public sealed class GetOrderQueryHandler(OrdersDbContext db) {
    public async Task<OrderDto?> Handle(Guid id) {
        var order = await db.Orders.FindAsync(id);
        if (order is null)
            return null;
        decimal total = order.Lines.Sum(l => l.Quantity * l.UnitPrice);
        return new OrderDto(order.Id, order.CustomerId, total);
    }
}
