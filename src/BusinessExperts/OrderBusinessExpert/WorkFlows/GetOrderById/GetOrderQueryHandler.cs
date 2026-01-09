using Domain= Experts.OrderBusinessExpert.Shared.Business.Domain;
using Experts.OrderBusinessExpert.Shared.Infrastructure.Data;

namespace Experts.OrderBusinessExpert.WorkFlows.GetOrderById;

public sealed class GetOrderQueryHandler(OrdersDbContext db) {
    public async Task<Domain. Order?> Handle(Guid id, CancellationToken token) {
        var infraOrder = await db.Orders.FindAsync([id], token);
        if (infraOrder is null)
            return null;

        var domainOrder = infraOrder.ToDomain();

        return domainOrder;
    }
}
