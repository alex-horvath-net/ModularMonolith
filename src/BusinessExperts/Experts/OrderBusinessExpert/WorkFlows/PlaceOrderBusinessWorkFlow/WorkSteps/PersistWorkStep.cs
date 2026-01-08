using Experts.Experts.OrderBusinessExpert.WorkFlows.PlaceOrderBusinessWorkFlow.Domain;
using Experts.Experts.OrderBusinessExpert.WorkFlows.PlaceOrderBusinessWorkFlow.Infrastructure.Data;
using Experts.Experts.OrderBusinessExpert.WorkFlows.PlaceOrderBusinessWorkFlow.Infrastructure.Data.Models;

namespace Experts.Experts.OrderBusinessExpert.WorkFlows.PlaceOrderBusinessWorkFlow.WorkSteps;

public class PersistWorkStep(OrdersDbContext db) {
    public async Task Save(Experts.Experts.OrderBusinessExpert.WorkFlows.PlaceOrderBusinessWorkFlow.Domain.Order order, CancellationToken token) {
        var data = Order.FromDomain(order);
        db.Add(order);
        await db.SaveChangesAsync(token);
    }
}
