using Microsoft.EntityFrameworkCore;
using Orders.Contracts;
using Orders.Infrastructure;

namespace Orders.Application;
internal sealed class OrderReadApi(OrdersDbContext db) : IOrderReadApi {
    public Task<OrderDto?> GetById(Guid id) =>
        db.Orders
          .Select(o => new OrderDto(o.Id, o.CustomerId, o.Lines.Sum(l => l.UnitPrice * l.Quantity)))
          .FirstOrDefaultAsync(o => o.Id == id);
}
