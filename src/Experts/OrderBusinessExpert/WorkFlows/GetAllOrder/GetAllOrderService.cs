using Business.Experts.OrderBusinessExpert.Shared.Business.Domain;
using Business.Experts.OrderBusinessExpert.Shared.Business.Services;
using Business.Experts.OrderBusinessExpert.Shared.Infrastructure.Data;

namespace Business.Experts.OrderBusinessExpert.WorkFlows.GetAllOrder;

internal sealed class GetAllOrderService(OrdersDbContext db) : IReadOrderService {
    public async Task<Order?> GetById(Guid id) {
        var infraOrder = await db.Orders.FindAsync(id);
        if (infraOrder is null)
            return null;
       var domainOrder = infraOrder.ToDomain();
        return domainOrder;
    }
}
