using Data = Experts.OrderExpert.Shared.Infrastructure.Data;

namespace Experts.OrderExpert.PlaceOrder;

public class StoreInfrastructure(Data.OrdersDbContext db) : BusinessWorkSteps.IStoreInfrastructure {
    public async Task Save(Data.Models.Order order, CancellationToken token) {
        db.Add(order);
        await db.SaveChangesAsync(token);
    }
}
