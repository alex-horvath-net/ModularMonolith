using BusinessExperts.OrderBusinessExpert.Shared.Business.Domain;
using BusinessExperts.OrderBusinessExpert.Shared.Infrastructure.Data;
using Domain = Business.Experts.OrderBusinessExpert.Shared.Business.Domain;
using Infra = Business.Experts.OrderBusinessExpert.Shared.Infrastructure.Data;

namespace BusinessExperts.OrderBusinessExpert.WorkFlows.PlaceOrderBusinessWorkFlow.WorkSteps;

public class PersistWorkStep(OrdersDbContext db) {
    public async Task Save(Order domainOrder, CancellationToken token) {
        var infraOrder = domainOrder.ToInfra();
        db.Add(infraOrder);
        await db.SaveChangesAsync(token);
    }  
}
    