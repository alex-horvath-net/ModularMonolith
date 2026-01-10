using Experts.OrderExpert.Shared.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Experts.OrderExpert.GetAll;

public sealed class GetAllOrderQueryHandler(OrdersDbContext db) {
    public async Task<List<Shared.Business.Domain.Order>> Handle(CancellationToken token = default) {
        var infraOrders = await db.Orders.AsNoTracking().ToListAsync(token);
        var ordersDomain = infraOrders.Select(order => order.ToDomain()).ToList();




        return ordersDomain;
    }
}
