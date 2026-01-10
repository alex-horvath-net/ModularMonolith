using BusinessExperts.OrderBusinessExpert.BusinessWorkFlows.Shared.Business.Domain;
using BusinessExperts.OrderBusinessExpert.BusinessWorkFlows.Shared.Infrastructure.Data;

namespace BusinessExperts.OrderBusinessExpert.BusinessWorkFlows.GetOrderById;

public sealed class GetOrderQueryHandler(OrdersDbContext db) {
    public async Task<Order?> Handle(Guid id, CancellationToken token) {
        var infraOrder = await db.Orders.FindAsync([id], token);
        if (infraOrder is null)
            return null;

        var domainOrder = infraOrder.ToDomain();

        return domainOrder;
    }
}
