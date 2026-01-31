using Experts.OrderExpert.Common.Business.Domain;
using Experts.OrderExpert.Common.Business.Services;
using Experts.OrderExpert.Common.Infrastructure.Data;

namespace Experts.OrderExpert.GetAllOrder;

internal sealed class GetAllOrderService(OrdersDbContext db) : IReadOrderService {
    public async Task<Order?> GetById(Guid id) {
        var infraOrder = await db.Orders.FindAsync(id);
        if (infraOrder is null)
            return null;
        var domainOrder = infraOrder.ToDomain();
        return domainOrder;
    }
}
