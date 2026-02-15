using Business.Experts.OrderExpert.Common.Business.Domain;
using Business.Experts.OrderExpert.Common.Business.Services;
using Business.Experts.OrderExpert.Common.Infrastructure.Data;

namespace Business.Experts.OrderExpert.GetAllOrder;

internal sealed class GetAllOrderService(OrdersDbContext db) : IReadOrderService {
    public async Task<Order?> GetById(Guid id) {
        var infraOrder = await db.Orders.FindAsync(id);
        if (infraOrder is null)
            return null;
        var domainOrder = infraOrder.ToDomain();
        return domainOrder;
    }
}
