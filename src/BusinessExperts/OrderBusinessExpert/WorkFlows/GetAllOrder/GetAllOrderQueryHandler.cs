using Experts.OrderBusinessExpert.Shared.Business.Domain;
using Experts.OrderBusinessExpert.Shared.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Experts.OrderBusinessExpert.WorkFlows.GetAllOrder;

public sealed class GetAllOrderQueryHandler(OrdersDbContext db) {
    public async Task<List<Order>> Handle(CancellationToken token = default) {
        return await db.Orders
            .AsNoTracking()
            .Select(o => new Order(
                o.Id,
                o.CustomerId,
                o.Lines.Sum(l => l.Quantity * l.UnitPrice)))
            .ToListAsync(token);
    }
}
