using Core.Features.Orders.Domain;
using Core.Features.Orders.Infrastructure.Data;
using Core.Features.Orders.Services;

namespace Core.Features.Orders.Slices.GetAllOrder;

internal sealed class GetAllOrderService(OrdersDbContext db) : IReadOrderService {
    public async Task<Order?> GetById(Guid id) {
        var infraOrder = await db.Orders.FindAsync(id);
        if (infraOrder is null)
            return null;
        var domainOrder = infraOrder.ToDomain();
        return domainOrder;
    }
}
