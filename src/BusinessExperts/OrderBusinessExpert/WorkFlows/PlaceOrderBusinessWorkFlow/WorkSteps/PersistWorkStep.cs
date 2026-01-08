using Experts.OrderBusinessExpert.Shared.Infrastructure.Data;
using Experts.OrderBusinessExpert.WorkFlows.PlaceOrderBusinessWorkFlow.Infrastructure.Data.Models;

namespace Experts.OrderBusinessExpert.WorkFlows.PlaceOrderBusinessWorkFlow.WorkSteps;

public class PersistWorkStep(OrdersDbContext db) {
    public async Task Save(Shared.Business.Domain.Order order, CancellationToken token) {
        var data = Infrastructure.Order.FromDomain(order);
        db.Add(order);
        await db.SaveChangesAsync(token);
    }
}
