using Business.OrderBusinessExpert.Contracts.DTOs;
using Business.OrderBusinessExpert.PlaceOrderBusinessWorkFlow.Infrastructure.Data;

namespace Business.OrderBusinessExpert.GetOrderById;

public sealed class GetOrderQueryHandler(OrdersDbContext db) {
    public async Task<OrderDto?> Handle(Guid id, CancellationToken token) {
        var order = await db.Orders.FindAsync([id], token);
        if (order is null)
            return null;
        decimal total = order.Lines.Sum(l => l.Quantity * l.UnitPrice);
        return new OrderDto(order.Id, order.CustomerId, total);
    }
}
