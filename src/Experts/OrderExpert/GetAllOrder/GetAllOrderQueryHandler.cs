using Experts.OrderExpert.Common.Business.Domain;
using Experts.OrderExpert.Common.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Experts.OrderExpert.GetAllOrder;

public sealed class GetAllOrderQueryHandler(OrdersDbContext db) {
    public async Task<List<Order>> Handle(CancellationToken token = default) {
        var infraOrders = await db.Orders.AsNoTracking().ToListAsync(token);
        var ordersDomain = infraOrders.Select(order => order.ToDomain()).ToList();




        return ordersDomain;
    }
}
