using Microsoft.EntityFrameworkCore;
using Orders.Create.Contracts.DTOs;
using Orders.Create.Infrastructure.Data;

namespace Orders.Create.Application.QueryHandlers;

public sealed class GetOrdersQueryHandler(OrdersDbContext db) {
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
