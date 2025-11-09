using Orders.Infrastructure;
using Orders.Contracts.DTOs;
using Microsoft.EntityFrameworkCore;

namespace Orders.Application;

public sealed class GetOrdersQueryHandler(OrdersDbContext db) {
    public async Task<List<OrderDto>> Handle() {
        var orders = await db.Orders.ToListAsync();
        var orderDtos = orders.Select(order=> new OrderDto(order.Id, order.CustomerId, order.Lines.Sum(l => l.Quantity * l.UnitPrice)));
        return orderDtos.ToList();
    }
}
