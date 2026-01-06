using BusinessExperts.Order.Contracts.DTOs;
using BusinessExperts.Order.CreateOrderWorkFlow.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BusinessExperts.Order.GetAllOrderWorkFlow;

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
