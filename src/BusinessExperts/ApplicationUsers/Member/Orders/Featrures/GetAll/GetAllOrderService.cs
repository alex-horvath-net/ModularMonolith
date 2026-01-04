using Business.ApplicationUsers.Member.Orders.Contracts.Abstraction;
using Business.ApplicationUsers.Member.Orders.Contracts.DTOs;
using Business.ApplicationUsers.Member.Orders.Featrures.Create.Infrastructure.Data;

namespace Business.ApplicationUsers.Member.Orders.Featrures.GetAll;

internal sealed class GetAllOrderService(OrdersDbContext db) : IReadOrderService {
    public async Task<OrderDto?> GetById(Guid id) {
        var order = await db.Orders.FindAsync(id);
        if (order is null)
            return null;
        decimal total = order.Lines.Sum(l => l.Quantity * l.UnitPrice);
        return new OrderDto(order.Id, order.CustomerId, total);
    }
}
