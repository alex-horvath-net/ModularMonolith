using Microsoft.EntityFrameworkCore;
using Orders.Core.Domain;
using Orders.Core.Infrastructure.Data;

namespace Orders.GetAllOrder;

public sealed class GetAllOrderQueryHandler(OrdersDbContext db) {
    public async Task<List<Order>> Handle(CancellationToken token = default) {
        var infraOrders = await db.Orders.AsNoTracking().ToListAsync(token);
        var ordersDomain = infraOrders.Select(order => order.ToDomain()).ToList();

        return ordersDomain;
    }
}
