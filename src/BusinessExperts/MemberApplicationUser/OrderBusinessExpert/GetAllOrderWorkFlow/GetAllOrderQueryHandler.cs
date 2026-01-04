using Business.MemberApplicationUser.OrderBusinessExpert.Contracts.DTOs;
using Business.MemberApplicationUser.OrderBusinessExpert.CreateOrderWorkFlow.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Business.MemberApplicationUser.OrderBusinessExpert.GetAllOrderWorkFlow;

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
