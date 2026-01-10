using Data = BusinessExperts.OrderBusinessExpert.BusinessWorkFlows.Shared.Infrastructure.Data;

namespace BusinessExperts.OrderBusinessExpert.BusinessWorkFlows.PlaceOrderBusinessWorkFlow.BusinessWorkSteps.StoreBusinessWorkSteps;

public class Infrastructure(Data.OrdersDbContext db) : Business.IInfrastructure {
    public async Task Save(Data.Models.Order order, CancellationToken token) {
        db.Add(order);
        await db.SaveChangesAsync(token);
    }
}

