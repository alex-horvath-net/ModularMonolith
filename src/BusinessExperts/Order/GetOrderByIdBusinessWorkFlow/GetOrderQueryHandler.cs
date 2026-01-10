using Experts.Order.Shared.Infrastructure.Data;

namespace Experts.Order.GetOrderByIdBusinessWorkFlow;

public sealed class GetOrderQueryHandler(OrdersDbContext db) {
    public async Task<Shared.Business.Domain.Order?> Handle(Guid id, CancellationToken token) {
        var infraOrder = await db.Orders.FindAsync([id], token);
        if (infraOrder is null)
            return null;

        var domainOrder = infraOrder.ToDomain();

        return domainOrder;
    }
}
