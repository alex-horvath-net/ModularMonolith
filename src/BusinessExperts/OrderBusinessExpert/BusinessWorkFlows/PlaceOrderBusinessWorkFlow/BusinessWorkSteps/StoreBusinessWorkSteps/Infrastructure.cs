using Experts.OrderBusinessExpert.BusinessWorkFlows.Shared.Infrastructure.Data;
using Experts.OrderBusinessExpert.BusinessWorkFlows.Shared.Infrastructure.Data.Models;
using Data = BusinessExperts.OrderBusinessExpert.BusinessWorkFlows.Shared.Infrastructure.Data;

namespace Experts.OrderBusinessExpert.BusinessWorkFlows.PlaceOrderBusinessWorkFlow.BusinessWorkSteps.StoreBusinessWorkSteps;

public class Infrastructure(OrdersDbContext db) : Business.IInfrastructure {
    public async Task Save(Order order, CancellationToken token) {
        db.Add(order);
        await db.SaveChangesAsync(token);
    }
}

