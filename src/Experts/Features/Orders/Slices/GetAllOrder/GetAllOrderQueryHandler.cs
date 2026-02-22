using Business.Features.Orders.Domain;
using Business.Features.Orders.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Business.Features.Orders.Slices.GetAllOrder;

public sealed class GetAllOrderQueryHandler(OrdersDbContext db) {
    public async Task<List<Order>> Handle(CancellationToken token = default) {
        var infraOrders = await db.Orders.AsNoTracking().ToListAsync(token);
        var ordersDomain = infraOrders.Select(order => order.ToDomain()).ToList();

        return ordersDomain;
    }
}
