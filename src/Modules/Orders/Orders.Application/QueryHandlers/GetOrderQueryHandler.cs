using Orders.Contracts.DTOs;
using Orders.Infrastructure;

namespace Orders.Application.QueryHandlers;

public sealed class GetOrderQueryHandler(OrdersDbContext db) {
    public async Task<OrderDto> Handle(Guid id) {
        var order = await db.Orders.FindAsync(id);
        var orderDto = new OrderDto(order.Id, order.CustomerId, order.Lines.Sum(x => x.Quantity * x.UnitPrice));
        return orderDto;
    }
} 
