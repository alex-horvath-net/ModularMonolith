using Microsoft.EntityFrameworkCore;
using Orders.Contracts.DTOs;
using Orders.Contracts.Services;
using Orders.Infrastructure;

namespace Orders.Application.QueryServices;
internal sealed class ReadOrderService(OrdersDbContext db) : IReadOrderService {
    public Task<OrderDto?> GetById(Guid id) =>
        db.Orders
          .Select(o => new OrderDto(o.Id, o.CustomerId, o.Lines.Sum(l => l.UnitPrice * l.Quantity)))
          .FirstOrDefaultAsync(o => o.Id == id);
}
 