using Business.Features.Orders.Domain;
using Business.Features.Orders.Infrastructure.Data;
using Business.Features.Orders.Services;

namespace Business.Features.Orders.Slices.GetAllOrder;

internal sealed class GetAllOrderService(OrdersDbContext db) : IReadOrderService {
    public async Task<Order?> GetById(Guid id) {
        var infraOrder = await db.Orders.FindAsync(id);
        if (infraOrder is null)
            return null;
        var domainOrder = infraOrder.ToDomain();
        return domainOrder;
    }
}
