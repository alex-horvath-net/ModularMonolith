using Domain = Experts.OrderBusinessExpert.Shared.Business.Domain;
using Infra = Experts.OrderBusinessExpert.Shared.Infrastructure.Data;

namespace Experts.OrderBusinessExpert.WorkFlows.PlaceOrderBusinessWorkFlow.WorkSteps;

public class PersistWorkStep(Infra.OrdersDbContext db) {
    public async Task Save(Domain.Order domainOrder, CancellationToken token) {
        var infraOrder = domainOrder.ToInfra();
        db.Add(infraOrder);
        await db.SaveChangesAsync(token);
    }  
}
    