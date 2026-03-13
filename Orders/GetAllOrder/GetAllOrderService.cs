using Orders.Core.Domain;
using Orders.Core.Infrastructure.Data;
using Orders.Core.Services;

namespace Orders.GetAllOrder;

internal sealed class GetAllOrderService(OrdersDbContext db) : IReadOrderService {
    public async Task<Order?> GetById(Guid id) {
        var infraOrder = await db.Orders.FindAsync(id);
        if (infraOrder is null)
            return null;
        var domainOrder = infraOrder.ToDomain();
        return domainOrder;
    }
}
