using Domain = Business.Experts.OrderBusinessExpert.Shared.Business.Domain;
using Business.Experts.OrderBusinessExpert.Shared.Infrastructure.Data;
using Business.Experts.OrderBusinessExpert.Shared.Business.Domain;

namespace Business.Experts.OrderBusinessExpert.WorkFlows.GetOrderById;

public sealed class GetOrderQueryHandler(OrdersDbContext db) {
    public async Task<Order?> Handle(Guid id, CancellationToken token) {
        var infraOrder = await db.Orders.FindAsync([id], token);
        if (infraOrder is null)
            return null;

        var domainOrder = infraOrder.ToDomain();

        return domainOrder;
    }
}
