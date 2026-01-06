using Business.OrderBusinessExpert.PlaceOrderBusinessWorkFlow.Infrastructure.Data;

namespace Business.OrderBusinessExpert.PlaceOrderBusinessWorkFlow.WorkSteps;

public class PersistWorkStep(OrdersDbContext db) {
    public async Task Save(Domain.Order order, CancellationToken token) {
        var data = Infrastructure.Data.Models.Order.FromDomain(order);
        db.Add(order);
        await db.SaveChangesAsync(token);
    }
}
