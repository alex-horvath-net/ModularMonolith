using Business.OrderBusinessExpert.Contracts.Abstraction;
using Business.OrderBusinessExpert.Contracts.DTOs;
using Business.OrderBusinessExpert.PlaceOrderBusinessWorkFlow.Infrastructure.Data;

namespace Business.OrderBusinessExpert.GetAllOrder;

internal sealed class GetAllOrderService(OrdersDbContext db) : IReadOrderService {
    public async Task<OrderDto?> GetById(Guid id) {
        var order = await db.Orders.FindAsync(id);
        if (order is null)
            return null;
        decimal total = order.Lines.Sum(l => l.Quantity * l.UnitPrice);
        return new OrderDto(order.Id, order.CustomerId, total);
    }
}
