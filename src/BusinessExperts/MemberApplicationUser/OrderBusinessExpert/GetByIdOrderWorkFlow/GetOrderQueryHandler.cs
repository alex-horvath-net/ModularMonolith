using Business.MemberApplicationUser.OrderBusinessExpert.Contracts.DTOs;
using Business.MemberApplicationUser.OrderBusinessExpert.CreateOrderWorkFlow.Infrastructure.Data;

namespace Business.MemberApplicationUser.OrderBusinessExpert.GetByIdOrderWorkFlow;

public sealed class GetOrderQueryHandler(OrdersDbContext db) {
    public async Task<OrderDto?> Handle(Guid id, CancellationToken token) {
        var order = await db.Orders.FindAsync([id], token);
        if (order is null)
            return null;
        decimal total = order.Lines.Sum(l => l.Quantity * l.UnitPrice);
        return new OrderDto(order.Id, order.CustomerId, total);
    }
}
