using Experts.OrderBusinessExpert.Shared.Business.Domain;
using Experts.OrderBusinessExpert.Shared.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Experts.OrderBusinessExpert.GetAllOrderBusinessWorkFlow;

public sealed class GetAllOrderQueryHandler(OrdersDbContext db) {
    public async Task<List<Order>> Handle(CancellationToken token = default) {
        var infraOrders = await db.Orders.AsNoTracking().ToListAsync(token);
        var ordersDomain = infraOrders.Select(order => order.ToDomain()).ToList();




        return ordersDomain;
    }
}
