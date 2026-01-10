using Experts.OrderBusinessExpert.Shared.Infrastructure.Data;
using Experts.OrderBusinessExpert.Shared.Infrastructure.Data.Models;

namespace Experts.OrderBusinessExpert.PlaceOrderBusinessWorkFlow.StoreBusinessWorkSteps;

public class Infrastructure(OrdersDbContext db) : Business.IInfrastructure {
    public async Task Save(Order order, CancellationToken token) {
        db.Add(order);
        await db.SaveChangesAsync(token);
    }
}

