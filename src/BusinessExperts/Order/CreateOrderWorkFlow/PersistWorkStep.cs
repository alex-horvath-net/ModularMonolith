using ApplicationUsers.Member.BusinessExperts.OrderBusinessExpert.CreateOrderWorkFlow.Infrastructure.Data.Models;
using BusinessExperts.Order.CreateOrderWorkFlow.Infrastructure.Data;

namespace BusinessExperts.Order.CreateOrderWorkFlow;

public class PersistWorkStep(OrdersDbContext db) {
    public async Task Save(Order order, CancellationToken token) {
        db.Add(order);
        await db.SaveChangesAsync(token);
    }
}
