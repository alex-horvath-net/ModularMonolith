using Experts.OrderExpert.Shared.Business.Domain;
using Experts.OrderExpert.Shared.Business.Services;
using Experts.OrderExpert.Shared.Infrastructure.Data;

namespace Experts.OrderExpert.GetAllOrderFlow;

internal sealed class GetAllOrderService(OrdersDbContext db) : IReadOrderService {
    public async Task<Order?> GetById(Guid id) {
        var infraOrder = await db.Orders.FindAsync(id);
        if (infraOrder is null)
            return null;
       var domainOrder = infraOrder.ToDomain();
        return domainOrder;
    }
}
