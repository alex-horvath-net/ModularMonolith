using BusinessExperts.OrderBusinessExpert.BusinessWorkFlows.Shared.Business.Domain;
using BusinessExperts.OrderBusinessExpert.BusinessWorkFlows.Shared.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BusinessExperts.OrderBusinessExpert.BusinessWorkFlows.GetAllOrder;

public sealed class GetAllOrderQueryHandler(OrdersDbContext db) {
    public async Task<List<Order>> Handle(CancellationToken token = default) {
        var infraOrders = await db.Orders.AsNoTracking().ToListAsync(token);
        var ordersDomain = infraOrders.Select(order => order.ToDomain()).ToList();




        return ordersDomain;
    }
}
