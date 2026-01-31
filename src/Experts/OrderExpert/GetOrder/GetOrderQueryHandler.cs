using Experts.OrderExpert.Common.Business.Domain;
using Experts.OrderExpert.Common.Infrastructure.Data;

namespace Experts.OrderExpert.GetOrder;

public sealed class GetOrderQueryHandler(OrdersDbContext db) {
    public async Task<Order?> Handle(Guid id, CancellationToken token) {
        var infraOrder = await db.Orders.FindAsync([id], token);
        if (infraOrder is null)
            return null;

        var domainOrder = infraOrder.ToDomain();

        return domainOrder;
    }
}
