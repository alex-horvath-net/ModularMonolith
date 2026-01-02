using BusinessExperts.Orders.Contracts.Abstraction;
using BusinessExperts.Orders.Contracts.DTOs;
using BusinessExperts.Orders.Featrures.Create.Infrastructure.Data;

namespace BusinessExperts.Orders.Featrures.GetAll;

internal sealed class GetAllOrderService(OrdersDbContext db) : IReadOrderService {
    public async Task<OrderDto?> GetById(Guid id) {
        var order = await db.Orders.FindAsync(id);
        if (order is null)
            return null;
        decimal total = order.Lines.Sum(l => l.Quantity * l.UnitPrice);
        return new OrderDto(order.Id, order.CustomerId, total);
    }
}
