using Experts.Experts.OrderBusinessExpert.Business.DTOs;
using Experts.Experts.OrderBusinessExpert.WorkFlows.PlaceOrderBusinessWorkFlow.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Experts.Experts.OrderBusinessExpert.WorkFlows.GetAllOrder;

public sealed class GetAllOrderQueryHandler(OrdersDbContext db) {
    public async Task<List<OrderDto>> Handle(CancellationToken token = default) {
        return await db.Orders
            .AsNoTracking()
            .Select(o => new OrderDto(
                o.Id,
                o.CustomerId,
                o.Lines.Sum(l => l.Quantity * l.UnitPrice)))
            .ToListAsync(token);
    }
}
