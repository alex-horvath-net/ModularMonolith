using Business.MemberApplicationUser.OrderBusinessExpert.CreateOrderWorkFlow.Infrastructure.Data;
using Business.MemberApplicationUser.OrderBusinessExpert.CreateOrderWorkFlow.Infrastructure.Data.Models;

namespace Business.MemberApplicationUser.OrderBusinessExpert.CreateOrderWorkFlow;

public class PersistWorkStep(OrdersDbContext db) {
    public async Task Save(Order order, CancellationToken token) {
        db.Add(order);
        await db.SaveChangesAsync(token);
    }
}
