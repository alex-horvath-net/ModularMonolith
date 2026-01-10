using BusinessExperts.OrderBusinessExpert.BusinessWorkFlows.Shared.Business.Domain;
using BusinessExperts.OrderBusinessExpert.BusinessWorkFlows.Shared.Business.Services;
using BusinessExperts.OrderBusinessExpert.BusinessWorkFlows.Shared.Infrastructure.Data;

namespace BusinessExperts.OrderBusinessExpert.BusinessWorkFlows.GetAllOrder;

internal sealed class GetAllOrderService(OrdersDbContext db) : IReadOrderService {
    public async Task<Order?> GetById(Guid id) {
        var infraOrder = await db.Orders.FindAsync(id);
        if (infraOrder is null)
            return null;
       var domainOrder = infraOrder.ToDomain();
        return domainOrder;
    }
}
