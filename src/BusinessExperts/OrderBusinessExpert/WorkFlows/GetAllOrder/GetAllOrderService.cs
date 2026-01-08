using Experts.OrderBusinessExpert.Shared.Business.Domain;
using Experts.OrderBusinessExpert.Shared.Business.Services;
using Experts.OrderBusinessExpert.Shared.Infrastructure.Data;

namespace Experts.OrderBusinessExpert.WorkFlows.GetAllOrder;

internal sealed class GetAllOrderService(OrdersDbContext db) : IReadOrderService {
    public async Task<Order?> GetById(Guid id) {
        var order = await db.Orders.FindAsync(id);
        if (order is null)
            return null;
        decimal total = order.Lines.Sum(l => l.Quantity * l.UnitPrice);
        return new Order(order.Id, order.CustomerId, total);
    }
}
