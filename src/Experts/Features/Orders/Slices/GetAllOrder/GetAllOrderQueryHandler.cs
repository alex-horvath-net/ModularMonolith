using Core.Features.Orders.Domain;
using Core.Features.Orders.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Core.Features.Orders.Slices.GetAllOrder;

public sealed class GetAllOrderQueryHandler(OrdersDbContext db) {
    public async Task<List<Order>> Handle(CancellationToken token = default) {
        var infraOrders = await db.Orders.AsNoTracking().ToListAsync(token);
        var ordersDomain = infraOrders.Select(order => order.ToDomain()).ToList();

        return ordersDomain;
    }
}
