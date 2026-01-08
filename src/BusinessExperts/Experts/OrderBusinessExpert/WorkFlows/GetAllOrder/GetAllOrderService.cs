using Experts.Experts.OrderBusinessExpert.Business.DTOs;
using Experts.Experts.OrderBusinessExpert.Contracts.Abstraction;
using Experts.Experts.OrderBusinessExpert.WorkFlows.PlaceOrderBusinessWorkFlow.Infrastructure.Data;

namespace Experts.Experts.OrderBusinessExpert.WorkFlows.GetAllOrder;

internal sealed class GetAllOrderService(OrdersDbContext db) : IReadOrderService {
    public async Task<OrderDto?> GetById(Guid id) {
        var order = await db.Orders.FindAsync(id);
        if (order is null)
            return null;
        decimal total = order.Lines.Sum(l => l.Quantity * l.UnitPrice);
        return new OrderDto(order.Id, order.CustomerId, total);
    }
}
