using Experts.Order.Shared.Business.Services;
using Experts.Order.Shared.Infrastructure.Data;

namespace Experts.Order.GetAll;

internal sealed class GetAllOrderService(OrdersDbContext db) : Shared.Business.Services.IReadOrderService {
    public async Task<Shared.Business.Domain.Order?> GetById(Guid id) {
        var infraOrder = await db.Orders.FindAsync(id);
        if (infraOrder is null)
            return null;
       var domainOrder = infraOrder.ToDomain();
        return domainOrder;
    }
}
