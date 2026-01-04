using Business.ApplicationUsers.Member.Orders.Contracts.DTOs;
using Business.ApplicationUsers.Member.Orders.Featrures.Create.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Business.ApplicationUsers.Member.Orders.Featrures.GetAll;

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
