using Domain = Experts.OrderBusinessExpert.Shared.Business.Domain;
using Data = Experts.OrderBusinessExpert.Shared.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Experts.OrderBusinessExpert.WorkFlows.GetAllOrder;

public sealed class GetAllOrderQueryHandler(Data.OrdersDbContext db) {
    public async Task<List<Domain.Order>> Handle(CancellationToken token = default) {
        var infraOrders = await db.Orders.AsNoTracking().ToListAsync(token);
        var ordersDomain = infraOrders.Select(order => order.ToDomain()).ToList();




        return ordersDomain;
    }
}
