using Business.MemberApplicationUser.OrderBusinessExpert.Contracts.Abstraction;
using Business.MemberApplicationUser.OrderBusinessExpert.Contracts.DTOs;
using Business.MemberApplicationUser.OrderBusinessExpert.CreateOrderWorkFlow.Infrastructure.Data;

namespace Business.MemberApplicationUser.OrderBusinessExpert.GetAllOrderWorkFlow;

internal sealed class GetAllOrderService(OrdersDbContext db) : IReadOrderService {
    public async Task<OrderDto?> GetById(Guid id) {
        var order = await db.Orders.FindAsync(id);
        if (order is null)
            return null;
        decimal total = order.Lines.Sum(l => l.Quantity * l.UnitPrice);
        return new OrderDto(order.Id, order.CustomerId, total);
    }
}
